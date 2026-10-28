using System.Globalization;
using System.Text.RegularExpressions;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class SubstituteService : ISubstituteService
    {
        private readonly ISubstituteRepository _substituteRepository;
        private readonly ICustomFieldRepository _customFieldRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<SubstituteService> _logger;

        public SubstituteService(
            ISubstituteRepository substituteRepository,
            ICustomFieldRepository customFieldRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILogger<SubstituteService> logger)
        {
            _substituteRepository = substituteRepository;
            _customFieldRepository = customFieldRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> AddSubstituteAsync(
            string className,
            int? originalTeacherId,
            int? substituteTeacherId,
            string? subject,
            int? lessonNumber,
            DateTime? originalDate,
            DateTime? substituteDate,
            string? oldRoom,
            string? room,
            string? note,
            IReadOnlyDictionary<int, string?>? customValues = null)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddSubstituteLesson);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa dodania zastępstwa: brak uprawnienia {Permission}", EnumPermissions.AddSubstituteLesson);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            className = Normalize(className) ?? string.Empty;
            subject = Normalize(subject);
            oldRoom = Normalize(oldRoom);
            room = Normalize(room);
            note = Normalize(note);

            if (className.Length == 0)
            {
                _notificationService.Show("Klasa nie może być pusta!", EnumNotificationType.Failure);
                return false;
            }
            if (className.Length > 50 || (subject?.Length ?? 0) > 100
                || (oldRoom?.Length ?? 0) > 50 || (room?.Length ?? 0) > 50 || (note?.Length ?? 0) > 500)
            {
                _notificationService.Show("Któreś z pól jest za długie!", EnumNotificationType.Failure);
                return false;
            }
            if (substituteDate == null)
            {
                _notificationService.Show("Podaj datę zastępstwa!", EnumNotificationType.Failure);
                return false;
            }
            if (originalTeacherId == null)
            {
                _notificationService.Show("Wybierz nauczyciela, którego zastępujemy!", EnumNotificationType.Failure);
                return false;
            }
            if (substituteTeacherId == null)
            {
                _notificationService.Show("Wybierz nauczyciela zastępującego!", EnumNotificationType.Failure);
                return false;
            }
            if (originalTeacherId == substituteTeacherId)
            {
                _notificationService.Show("Nauczyciel zastępujący musi być inną osobą!", EnumNotificationType.Failure);
                return false;
            }
            if (lessonNumber is < 0 or > 20)
            {
                _notificationService.Show("Nieprawidłowy numer lekcji!", EnumNotificationType.Failure);
                return false;
            }

            var fieldValues = await BuildFieldValuesAsync(customValues);
            if (fieldValues == null)
            {
                return false;
            }

            var substitute = new SubstituteLessons
            {
                ClassName = className,
                OriginalTeacherId = originalTeacherId,
                SubstituteTeacherId = substituteTeacherId,
                Subject = subject,
                LessonNumber = lessonNumber,
                // Gdy nie podano oryginalnej daty, lekcja odbywała się w dniu zastępstwa.
                OriginalDate = (originalDate ?? substituteDate.Value).Date,
                SubstituteDate = substituteDate.Value.Date,
                OldRoom = oldRoom,
                Room = room,
                Note = note,
                CreatedDate = DateTime.Now,
                FieldValues = fieldValues
            };

            var success = await _substituteRepository.AddSubstituteAsync(substitute);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} dodał zastępstwo {SubstituteId} (klasa {ClassName}, data {SubstituteDate:yyyy-MM-dd})",
                    actorId, substitute.Id, substitute.ClassName, substitute.SubstituteDate);
                _notificationService.Show("Dodano zastępstwo!", EnumNotificationType.Success);
            }
            else
            {
                _logger.LogError("Nie udało się dodać zastępstwa (klasa {ClassName})", className);
                _notificationService.Show("Dodanie zastępstwa nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<bool> RemoveSubstituteAsync(int id)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddSubstituteLesson);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa usunięcia zastępstwa {SubstituteId}: brak uprawnienia {Permission}", id, EnumPermissions.AddSubstituteLesson);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _substituteRepository.RemoveSubstituteAsync(id);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} usunął zastępstwo {SubstituteId}", actorId, id);
                _notificationService.Show("Usunięto zastępstwo!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Usunięcie zastępstwa nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<List<SubstituteLessons>> GetUpcomingSubstitutesAsync()
        {
            return await _substituteRepository.GetUpcomingSubstitutesAsync(DateTime.Today);
        }

        public async Task<List<SubstituteLessons>> GetRecentSubstitutesAsync(int count = 50)
        {
            return await _substituteRepository.GetRecentSubstitutesAsync(count);
        }

        // Sprawdza wartości własnych pól względem definicji; zwraca null (z komunikatem), gdy któraś jest niepoprawna.
        private async Task<List<SubstituteFieldValue>?> BuildFieldValuesAsync(IReadOnlyDictionary<int, string?>? customValues)
        {
            var result = new List<SubstituteFieldValue>();
            var fields = await _customFieldRepository.GetActiveAsync();
            foreach (var field in fields)
            {
                string? raw = null;
                customValues?.TryGetValue(field.Id, out raw);
                var value = Normalize(raw);

                if (field.FieldType == EnumCustomFieldType.Checkbox)
                {
                    result.Add(new SubstituteFieldValue { CustomFieldId = field.Id, Value = value == "true" ? "true" : "false" });
                    continue;
                }
                if (value == null)
                {
                    if (field.IsRequired)
                    {
                        _notificationService.Show($"Pole „{field.Label}” jest wymagane!", EnumNotificationType.Failure);
                        return null;
                    }
                    continue;
                }
                if (value.Length > 500)
                {
                    _notificationService.Show($"Pole „{field.Label}” jest za długie (max 500 znaków)!", EnumNotificationType.Failure);
                    return null;
                }

                switch (field.FieldType)
                {
                    case EnumCustomFieldType.Number:
                        if (!decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                        {
                            _notificationService.Show($"Pole „{field.Label}” musi być liczbą!", EnumNotificationType.Failure);
                            return null;
                        }
                        break;
                    case EnumCustomFieldType.Date:
                        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        {
                            _notificationService.Show($"Pole „{field.Label}” musi być datą!", EnumNotificationType.Failure);
                            return null;
                        }
                        value = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        break;
                    case EnumCustomFieldType.Select:
                        if (!field.GetOptions().Contains(value))
                        {
                            _notificationService.Show($"Wybierz poprawną opcję w polu „{field.Label}”!", EnumNotificationType.Failure);
                            return null;
                        }
                        break;
                }

                result.Add(new SubstituteFieldValue { CustomFieldId = field.Id, Value = value });
            }
            return result;
        }

        private static string? Normalize(string? value)
        {
            var result = Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ");
            return result.Length == 0 ? null : result;
        }
    }
}
