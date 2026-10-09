using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly NewspaperDbContext _context;
        public ArticleRepository(NewspaperDbContext context)
        {
            _context = context;
        }

        public async Task AddArticleAsync(Article article)
        {
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
           
        }
        public async Task<Article> GetArticleByIdAsync(int id)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
            {
                throw new Exception($"Article with ID {id} not found.");
            }
            return article;
        }
        public async Task RemoveArticleByIdAsync(int id)
        {
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
