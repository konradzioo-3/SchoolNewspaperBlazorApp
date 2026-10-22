using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IArticleService
    {
        Task<int> AddArticleAsync(string title, string text, string author, int id, EnumArticleType? articleType);
        Task<List<Article>> GetLatestArticlesAsync();
        Task<Article> GetArticleByIdAsync(int id);
        Task<bool> RemoveArticleByIdAsync(int id);
        Task UpdateArticleByIdAsync(int id,string title, string text, string author, int fileId,EnumArticleType? articleType);
        Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType);

    }
}
