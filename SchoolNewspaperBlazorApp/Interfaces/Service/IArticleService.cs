using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IArticleService
    {
        Task<int> AddArticleAsync(string title, string text, int id, EnumArticleType? articleType);
        Task<List<Article>> GetLatestArticlesAsync();
        Task<Article> GetArticleByIdAsync(int id);
        Task<bool> RemoveArticleByIdAsync(int id);
        Task<bool> UpdateArticleByIdAsync(int id,string title, string text,  int fileId,EnumArticleType? articleType);
        Task<List<ArticleBlock>> GetBlocksAsync(int articleId);
        Task<bool> SaveBlocksAsync(int articleId, List<ArticleBlock> blocks);
        Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType);

    }
}
