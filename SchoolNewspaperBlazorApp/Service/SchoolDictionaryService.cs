using System.Text.RegularExpressions;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class SchoolDictionaryService : ISchoolDictionaryService
    {
        private readonly ISchoolDictionaryRepository _repository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<SchoolDictionaryService> _logger;

        public SchoolDictionaryService(
            ISchoolDictionaryRepository repository,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILogger<SchoolDictionaryService> logger)
        {
            _repository = repository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task<List<DictionaryItem>> GetAllAsync(EnumDictionaryKind kind) => _repository.GetAllAsync(kind);

        public async Task<bool> AddAsync(EnumDictionaryKind kind, string name)
        {
            var permission = GetPermission(kind);
            if (!await _permissionService.HasPermissionAsync(permission))
            {
                _logger.LogWarning("Odmowa dodania wpisu ({Kind}): brak uprawnienia {Permission}", kind, permission);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            name = Normalize(name);
            var label = GetLabel(kind);
            if (name.Length == 0)
            {
                _notificationService.Show($"Nazwa ({label}) nie może być pusta!", EnumNotificationType.Failure);
                return false;
            }
            if (name.Length > GetMaxLength(kind))
            {
                _notificationService.Show($"Nazwa ({label}) jest za długa (max {GetMaxLength(kind)} znaków)!", EnumNotificationType.Failure);
                return false;
            }
            if (await _repository.ExistsAsync(kind, name))
            {
                _notificationService.Show($"Taki wpis ({label}) już istnieje!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _repository.AddAsync(kind, name);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} dodał wpis ({Kind}): {Name}", actorId, kind, name);
                _notificationService.Show($"Dodano: {label.ToLower()} {name}", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Dodanie nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<bool> RemoveAsync(EnumDictionaryKind kind, int id)
        {
            var permission = GetPermission(kind);
            if (!await _permissionService.HasPermissionAsync(permission))
            {
                _logger.LogWarning("Odmowa usunięcia wpisu ({Kind}) {Id}: brak uprawnienia {Permission}", kind, id, permission);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _repository.RemoveAsync(kind, id);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} usunął wpis ({Kind}) {Id}", actorId, kind, id);
                _notificationService.Show("Usunięto wpis!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Usunięcie nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        private static EnumPermissions GetPermission(EnumDictionaryKind kind) => kind switch
        {
            EnumDictionaryKind.Class => EnumPermissions.ManageClasses,
            EnumDictionaryKind.Room => EnumPermissions.ManageRooms,
            EnumDictionaryKind.Subject => EnumPermissions.ManageSubjects,
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        private static string GetLabel(EnumDictionaryKind kind) => kind switch
        {
            EnumDictionaryKind.Class => "Klasa",
            EnumDictionaryKind.Room => "Sala",
            EnumDictionaryKind.Subject => "Przedmiot",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        private static int GetMaxLength(EnumDictionaryKind kind) => kind == EnumDictionaryKind.Subject ? 100 : 50;

        private static string Normalize(string? value)
        {
            return Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ");
        }
    }
}
