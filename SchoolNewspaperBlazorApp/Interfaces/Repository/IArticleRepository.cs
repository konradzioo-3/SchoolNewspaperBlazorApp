using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IArticleRepository
    {
        Task<int> AddArticleAsync(Article article);
        Task<List<Article>> GetLatestArticlesAsync();
        Task<Article> GetArticleByIdAsync(int id);
        Task<bool> RemoveArticleByIdAsync(int id);
        Task<int> UpdateArticleByIdAsync(Article article);
        Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType);
    }
}
