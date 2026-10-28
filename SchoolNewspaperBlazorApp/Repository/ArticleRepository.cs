using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<ArticleRepository> _logger;

        public ArticleRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<ArticleRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<int> UpdateArticleByIdAsync(Article article)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(article.Title))
            {
                _logger.LogWarning("Edycja artykułu {ArticleId} odrzucona: pusty tytuł", article.Id);
                return 0;
            }
            if (string.IsNullOrWhiteSpace(article.Text))
            {
                _logger.LogWarning("Edycja artykułu {ArticleId} odrzucona: pusta treść", article.Id);
                return 0;
            }

            try
            {
                var existingArticle = await _context.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
                if (existingArticle == null)
                {
                    _logger.LogWarning("Edycja artykułu nie powiodła się: artykuł {ArticleId} nie istnieje", article.Id);
                    return 0;
                }
                if (article.MainImageId != 0)
                {
                    existingArticle.MainImageId = article.MainImageId;
                }
                existingArticle.Title = article.Title;
                existingArticle.Text = article.Text;
                // Wcześniej zmiana kategorii przy edycji nie była zapisywana.
                existingArticle.ArticleType = article.ArticleType;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Zaktualizowano artykuł {ArticleId}", article.Id);
                return article.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas edycji artykułu {ArticleId}", article.Id);
                return 0;
            }
        }

        public async Task<List<ArticleBlock>> GetBlocksAsync(int articleId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.ArticleBlocks
                .AsNoTracking()
                .Where(b => b.ArticleId == articleId)
                .OrderBy(b => b.Position)
                .ToListAsync();
        }

        // Zapisuje całą treść naraz: stare bloki znikają, nowe dostają kolejność 0, 1, 2...
        public async Task<bool> ReplaceBlocksAsync(int articleId, List<ArticleBlock> blocks)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var old = await _context.ArticleBlocks.Where(b => b.ArticleId == articleId).ToListAsync();
                _context.ArticleBlocks.RemoveRange(old);

                for (var i = 0; i < blocks.Count; i++)
                {
                    blocks[i].Id = 0;
                    blocks[i].ArticleId = articleId;
                    blocks[i].Position = i;
                }
                await _context.ArticleBlocks.AddRangeAsync(blocks);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Zapisano {Count} bloków treści artykułu {ArticleId}", blocks.Count, articleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zapisu bloków treści artykułu {ArticleId}", articleId);
                return false;
            }
        }

        public async Task<int> AddArticleAsync(Article article)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(article.Title))
            {
                _logger.LogWarning("Dodanie artykułu odrzucone: pusty tytuł");
                return 0;
            }
            if (string.IsNullOrWhiteSpace(article.Text))
            {
                _logger.LogWarning("Dodanie artykułu odrzucone: pusta treść");
                return 0;
            }
            try
            {
                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania artykułu");
                return 0;
            }
            _logger.LogInformation("Dodano artykuł {ArticleId}", article.Id);
            return article.Id;
        }

        public async Task<List<Article>> GetLatestArticlesAsync()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Articles
                .OrderByDescending(a => a.PublishDate)
                .Take(5)
                .ToListAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
            {
                _logger.LogWarning("Nie znaleziono artykułu {ArticleId}", id);
                throw new Exception($"Article with ID {id} not found.");
            }
            return article;
        }

        // Soft delete: nie kasujemy wiersza, tylko ustawiamy IsDeleted = 1.
        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
                if (article == null)
                {
                    _logger.LogWarning("Usunięcie artykułu nie powiodło się: artykuł {ArticleId} nie istnieje", id);
                    return false;
                }

                article.IsDeleted = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Usunięto (soft delete) artykuł {ArticleId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania artykułu {ArticleId}", id);
                return false;
            }
        }

        public async Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Articles
                .Where(a => a.ArticleType == articleType)
                .OrderByDescending(a => a.PublishDate)
                .ToListAsync();
        }
    }
}
