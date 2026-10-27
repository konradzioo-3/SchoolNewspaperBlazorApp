using System.Text.RegularExpressions;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TeacherService> _logger;

        public TeacherService(
            ITeacherRepository teacherRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILogger<TeacherService> logger)
        {
            _teacherRepository = teacherRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _teacherRepository.GetAllTeachersAsync();
        }

        public async Task<Teacher?> AddTeacherAsync(string name, string surname)
        {
            if (!await CanManageTeachersAsync())
            {
                _logger.LogWarning("Odmowa dodania nauczyciela: brak uprawnienia {Permission}", EnumPermissions.ManageTeachers);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return null;
            }

            name = Normalize(name);
            surname = Normalize(surname);
            if (name.Length == 0 || surname.Length == 0)
            {
                _notificationService.Show("Imię i nazwisko nauczyciela nie mogą być puste!", EnumNotificationType.Failure);
                return null;
            }
            if (name.Length > 100 || surname.Length > 100)
            {
                _notificationService.Show("Imię lub nazwisko jest za długie (max 100 znaków)!", EnumNotificationType.Failure);
                return null;
            }
            if (await _teacherRepository.TeacherExistsAsync(name, surname))
            {
                _notificationService.Show("Taki nauczyciel już istnieje!", EnumNotificationType.Failure);
                return null;
            }

            var teacher = await _teacherRepository.AddTeacherAsync(new Teacher { Name = name, Surname = surname });
            if (teacher != null)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} dodał nauczyciela {TeacherId}", actorId, teacher.Id);
                _notificationService.Show("Dodano nauczyciela!", EnumNotificationType.Success);
            }
            else
            {
                _logger.LogError("Nie udało się dodać nauczyciela");
                _notificationService.Show("Dodanie nauczyciela nie powiodło się!", EnumNotificationType.Error);
            }
            return teacher;
        }

        public async Task<bool> RemoveTeacherAsync(int id)
        {
            if (!await CanManageTeachersAsync())
            {
                _logger.LogWarning("Odmowa usunięcia nauczyciela {TeacherId}: brak uprawnienia {Permission}", id, EnumPermissions.ManageTeachers);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _teacherRepository.ArchiveTeacherAsync(id);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} usunął nauczyciela {TeacherId}", actorId, id);
                _notificationService.Show("Usunięto nauczyciela!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Usunięcie nauczyciela nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        // Nauczyciela może dodać ten, kto zarządza nauczycielami, a także (jak dotąd) ten, kto dodaje zastępstwa.
        private async Task<bool> CanManageTeachersAsync()
        {
            return await _permissionService.HasPermissionAsync(EnumPermissions.ManageTeachers)
                || await _permissionService.HasPermissionAsync(EnumPermissions.AddSubstituteLesson);
        }

        private static string Normalize(string? value)
        {
            return Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ");
        }
    }
}
