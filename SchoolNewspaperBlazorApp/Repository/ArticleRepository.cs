using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;

        public ArticleRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> UpdateArticleByIdAsync(Article article)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(article.Title))
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(article.Text))
            {
                return 0;
            }

                try
                {

                var existingArticle = await _context.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
                if (existingArticle == null)
                {
                        return 0;

                    }
                    if (article.MainImageId != 0)
                {
                    existingArticle.MainImageId = article.MainImageId;
                }
                existingArticle.Title = article.Title;
                existingArticle.Text = article.Text;
                existingArticle.Author = article.Author;

                await _context.SaveChangesAsync();
                return article.Id;
            }
            catch (Exception)
            {
                return 0; 
            }


        }
        public async Task<int> AddArticleAsync(Article article)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            if(string.IsNullOrWhiteSpace(article.Title))
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(article.Text))
            {
                return 0;
            }
            try
            {
                
                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
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
                throw new Exception($"Article with ID {id} not found.");
            }
            return article;
        }
        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
                if(article != null)
                {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
                return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }




        }
        public async Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Articles.Where(a => a.ArticleType == articleType).ToListAsync();
        }
    }
}
