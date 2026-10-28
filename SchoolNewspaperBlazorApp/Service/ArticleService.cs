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
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(
            IArticleRepository articleRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            IAuthRepository authRepository,
            ILogger<ArticleService> logger)
        {
            _articleRepository = articleRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<int> AddArticleAsync(string title, string text, int fileId, EnumArticleType? articleType)
        {
            if (string.IsNullOrEmpty(title))
            {
                _notificationService.Show("Tytuł nie może być pusty!", EnumNotificationType.Failure);
                return 0;
            }
            if (string.IsNullOrEmpty(text))
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
                _logger.LogWarning("Odmowa dodania artykułu: brak uprawnienia {Permission}", EnumPermissions.AddArticle);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return 0;
            }
            var userId = await _permissionService.GetUserId();
            var author = await _authRepository.GetUserByIdAsync(userId);
            var article = new Article
            {
                Title = title,
                Text = text,
                Author = $"{author.Name} {author.Surname}",
                AuthorRole = author.Role?.Name,
                PublishDate = DateTime.Now,
                MainImageId = fileId,
                ArticleType = articleType.Value
            };
            var articleId = await _articleRepository.AddArticleAsync(article);
            if (articleId != 0)
            {
                _logger.LogInformation("Użytkownik {UserId} dodał artykuł {ArticleId} (kategoria {ArticleType})", userId, articleId, articleType.Value);
                _notificationService.Show("Pomyślnie dodano artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _logger.LogError("Użytkownik {UserId} nie zdołał dodać artykułu", userId);
                _notificationService.Show("Podczas dodawania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
            }
            return articleId;
        }

        public async Task<bool> UpdateArticleByIdAsync(int id, string title, string text, int fileId, EnumArticleType? articleType)
        {
            if (string.IsNullOrEmpty(title))
            {
                _notificationService.Show("Tytuł nie może być pusty!", EnumNotificationType.Failure);
                return false;
            }
            if (string.IsNullOrEmpty(text))
            {
                _notificationService.Show("Treść nie może być pusta!", EnumNotificationType.Failure);
                return false;
            }
            if (articleType == null)
            {
                _notificationService.Show("Musisz wybrac kategorię!", EnumNotificationType.Failure);
                return false;
            }
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.EditArticle);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa edycji artykułu {ArticleId}: brak uprawnienia {Permission}", id, EnumPermissions.EditArticle);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }
            var article = new Article
            {
                Id = id,
                Title = title,
                Text = text,
                // Autor i rola są ustawiane raz, przy tworzeniu artykułu. Edycja ich nie zmienia
                // (repozytorium ignoruje to pole) - poprawić je można tylko w SQL.
                Author = string.Empty,
                MainImageId = fileId,
                ArticleType = articleType.Value
            };
            var success = await _articleRepository.UpdateArticleByIdAsync(article);
            if (success != 0)
            {
                _logger.LogInformation("Zaktualizowano artykuł {ArticleId}", id);
                _notificationService.Show("Pomyślnie zaaktualizowano artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _logger.LogError("Nie udało się zaktualizować artykułu {ArticleId}", id);
                _notificationService.Show("Podczas edytowania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
            }
            return success != 0;
        }

        public Task<List<ArticleBlock>> GetBlocksAsync(int articleId) => _articleRepository.GetBlocksAsync(articleId);

        public async Task<bool> SaveBlocksAsync(int articleId, List<ArticleBlock> blocks)
        {
            // Treść zapisuje ten, kto może dodawać lub edytować artykuły.
            bool allowed = await _permissionService.HasPermissionAsync(EnumPermissions.AddArticle)
                || await _permissionService.HasPermissionAsync(EnumPermissions.EditArticle);
            if (!allowed)
            {
                _logger.LogWarning("Odmowa zapisu treści artykułu {ArticleId}: brak uprawnień", articleId);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }
            foreach (var block in blocks)
            {
                if (block.BlockType == EnumArticleBlockType.Text && (block.Text?.Length ?? 0) > 50000)
                {
                    _notificationService.Show("Jeden z bloków tekstu jest za długi (max 50 000 znaków)!", EnumNotificationType.Failure);
                    return false;
                }
            }
            var success = await _articleRepository.ReplaceBlocksAsync(articleId, blocks);
            if (!success)
            {
                _notificationService.Show("Nie udało się zapisać treści artykułu!", EnumNotificationType.Error);
            }
            return success;
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
                _logger.LogWarning("Nie znaleziono artykułu {ArticleId}", id);
                throw new Exception($"Article with ID {id} not found.");
            }
            return article;
        }

        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.DeleteArticle);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa usunięcia artykułu {ArticleId}: brak uprawnienia {Permission}", id, EnumPermissions.DeleteArticle);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }
            if (id <= 0)
            {
                _logger.LogWarning("Nieprawidłowe ID artykułu do usunięcia: {ArticleId}", id);
                _notificationService.Show("Podczas usuwania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
                return false;
            }

            var success = await _articleRepository.RemoveArticleByIdAsync(id);
            if (success)
            {
                var userId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {UserId} usunął artykuł {ArticleId}", userId, id);
                _notificationService.Show("Pomyślnie usunięto artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Podczas usuwania artykułu wystąpił bład! Skontaktuj się z obłusga techniczna!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<List<Article>> GetAllArticleByTypeAsync(EnumArticleType articleType)
        {
            return await _articleRepository.GetAllArticleByTypeAsync(articleType);
        }
    }
}
