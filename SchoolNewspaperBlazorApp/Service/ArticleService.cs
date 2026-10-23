using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IAuthRepository _authRepository;
        public ArticleService(IArticleRepository articleRepository, IPermissionService permissionService,INotificationService notificationService,IAuthRepository authRepository)
        {
            _articleRepository = articleRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _authRepository = authRepository;
        }
        public async Task<int> AddArticleAsync(string title, string text,int fileId,EnumArticleType? articleType)
        {
        
            if(string.IsNullOrEmpty(title))
            {
                _notificationService.Show("Tytuł nie może być pusty!", EnumNotificationType.Failure);
                return 0;
            }
            if(string.IsNullOrEmpty(text))
            {
                _notificationService.Show("Treść nie może być pusta!", EnumNotificationType.Failure);
                return 0;
            }
            if (articleType == null)
            {
                _notificationService.Show("Musisz wybrac kategorię!", EnumNotificationType.Failure);
                return 0;
            }
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddArticle);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return 0;
            }
                var userId = await _permissionService.GetUserId();
                var author = await _authRepository.GetUserByIdAsync(userId);
                var article = new Article
                {
                    Title = title,
                    Text = text,
                    Author = $"{author.Name} {author.Surname}\n{author.Role}",
                    PublishDate = DateTime.Now,
                    MainImageId = fileId,
                    ArticleType = articleType.Value
                };
            var articleId = await _articleRepository.AddArticleAsync(article);
            if(articleId != 0)
            {
                _notificationService.Show("Pomyślnie dodano artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Podczas dodawania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
            }
            return articleId;
        }
        public async Task UpdateArticleByIdAsync(int id,string title,string text,int fileId,EnumArticleType? articleType)
        {
            if (string.IsNullOrEmpty(title))
            {
                _notificationService.Show("Tytuł nie może być pusty!", EnumNotificationType.Failure);
                return;
            }
            if (string.IsNullOrEmpty(text))
            {
                _notificationService.Show("Treść nie może być pusta!", EnumNotificationType.Failure);
                return;
            }
            if (articleType == null)
            {
                _notificationService.Show("Musisz wybrac kategorię!", EnumNotificationType.Failure);
                return;
            }
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.EditArticle);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return;
            }
            var userId = await _permissionService.GetUserId();
            var author = await _authRepository.GetUserByIdAsync(userId);
            var article = new Article
            {
                Id = id,
                Title = title,
                Text = text,
                Author = $"{author.Name} {author.Surname}\n{author.Role}",
                MainImageId = fileId,
                ArticleType = articleType.Value
                
            };
           var success =  await _articleRepository.UpdateArticleByIdAsync(article);
            if(success != 0)
            {
                _notificationService.Show("Pomyślnie zaaktualizowano artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Podczas edytowania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
            }
        }
        public async Task<List<Article>> GetLatestArticlesAsync()
        {
            return await _articleRepository.GetLatestArticlesAsync();
        }
        public async Task<Article> GetArticleByIdAsync(int id)
        {
            Article article = await _articleRepository.GetArticleByIdAsync(id);
            if (article == null)
            {
                throw new Exception($"Article with ID {id} not found.");
            }
            return article;
        }
        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.DeleteArticle);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }
            if (id > 0)
            {
                _notificationService.Show("Pomyślnie usunięto artykuł!", EnumNotificationType.Success);
                await _articleRepository.RemoveArticleByIdAsync(id);
                return true;
            }
            else
            {
            _notificationService.Show("Podczas usuwania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
            return false;
            }
        }
        public async Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType)
        {
            return await _articleRepository.GetAllArticleByTypeAsync(articleType);
        }
    }
}
