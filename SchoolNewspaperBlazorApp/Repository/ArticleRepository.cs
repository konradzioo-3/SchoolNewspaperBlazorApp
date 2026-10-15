using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SchoolNewspaperBlazorApp.Data;
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

        public async Task AddArticleAsync(Article article)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Article>> GetAllArticlesAsync()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.Articles.ToListAsync();
           
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
        public async Task RemoveArticleByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article == null) {
                throw new Exception($"Article with ID {id} not found.");
            }
            else
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
                

        }
    }
}
