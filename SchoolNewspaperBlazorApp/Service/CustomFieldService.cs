using System.Text.RegularExpressions;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class CustomFieldService : ICustomFieldService
    {
        private readonly ICustomFieldRepository _repository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CustomFieldService> _logger;

        public CustomFieldService(
            ICustomFieldRepository repository,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILogger<CustomFieldService> logger)
        {
            _repository = repository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task<List<CustomField>> GetActiveAsync() => _repository.GetActiveAsync();

        public async Task<bool> AddAsync(string label, EnumCustomFieldType type, string? options, bool isRequired, bool showOnList)
        {
            if (!await _permissionService.HasPermissionAsync(EnumPermissions.ManageCustomFields))
            {
                _logger.LogWarning("Odmowa dodania pola formularza: brak uprawnienia {Permission}", EnumPermissions.ManageCustomFields);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            label = Regex.Replace((label ?? string.Empty).Trim(), @"\s+", " ");
            if (label.Length == 0)
            {
                _notificationService.Show("Nazwa pola nie może być pusta!", EnumNotificationType.Failure);
                return false;
            }
            if (label.Length > 100)
            {
                _notificationService.Show("Nazwa pola jest za długa (max 100 znaków)!", EnumNotificationType.Failure);
                return false;
            }
            var existing = await _repository.GetActiveAsync();
            if (existing.Any(f => string.Equals(f.Label, label, StringComparison.OrdinalIgnoreCase)))
            {
                _notificationService.Show("Pole o takiej nazwie już istnieje!", EnumNotificationType.Failure);
                return false;
            }

            string? normalizedOptions = null;
            if (type == EnumCustomFieldType.Select)
            {
                var list = (options ?? string.Empty)
                    .Split(new[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                if (list.Count < 2)
                {
                    _notificationService.Show("Lista wyboru musi mieć co najmniej dwie opcje!", EnumNotificationType.Failure);
                    return false;
                }
                if (list.Any(o => o.Length > 100))
                {
                    _notificationService.Show("Opcja listy jest za długa (max 100 znaków)!", EnumNotificationType.Failure);
                    return false;
                }
                normalizedOptions = string.Join(";", list);
                if (normalizedOptions.Length > 1000)
                {
                    _notificationService.Show("Lista opcji jest za długa!", EnumNotificationType.Failure);
                    return false;
                }
            }

            var field = new CustomField
            {
                Label = label,
                FieldType = type,
                Options = normalizedOptions,
                // Pole typu "pole wyboru" jest zawsze zapisywane jako tak/nie, więc nie może być "wymagane".
                IsRequired = isRequired && type != EnumCustomFieldType.Checkbox,
                ShowOnList = showOnList
            };

            var success = await _repository.AddAsync(field);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} dodał pole formularza {FieldId} ({Type})", actorId, field.Id, type);
                _notificationService.Show("Dodano pole formularza!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Dodanie pola nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            if (!await _permissionService.HasPermissionAsync(EnumPermissions.ManageCustomFields))
            {
                _logger.LogWarning("Odmowa usunięcia pola formularza {FieldId}: brak uprawnienia {Permission}", id, EnumPermissions.ManageCustomFields);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _repository.RemoveAsync(id);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} usunął pole formularza {FieldId}", actorId, id);
                _notificationService.Show("Usunięto pole formularza!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Usunięcie pola nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }
    }
}
