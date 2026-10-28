using Microsoft.AspNetCore.Identity;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Text.RegularExpressions;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<AuthService> _logger;
        private static readonly Regex EmailRegex = new(
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");

        public AuthService(
            IAuthRepository authRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            IRoleRepository roleRepository,
            ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }

        public async Task<bool> AddUserAsync(string password, string email, string name, string surname, int? roleId)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _notificationService.Show("Hasło jest puste!", EnumNotificationType.Failure);
                return false;
            }
            if (!EmailRegex.IsMatch(email))
            {
                _notificationService.Show("E-mail jest niepoprawny!", EnumNotificationType.Failure);
                return false;
            }
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddUser);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa dodania użytkownika: brak uprawnienia {Permission}", EnumPermissions.AddUser);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            if (roleId == null || await _roleRepository.GetRoleByIdAsync(roleId.Value) == null)
            {
                _notificationService.Show("Wybierz stanowisko z listy!", EnumNotificationType.Failure);
                return false;
            }

            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);
            var auth = new User
            {
                Password = passwordHash,
                Email = email,
                RegisterDate = DateTime.Now,
                // Hasło ustawia administrator - użytkownik wybiera własne przy pierwszym logowaniu.
                MustChangePassword = true,
                Name = name,
                Surname = surname,
                RoleId = roleId.Value
            };
            var succes = await _authRepository.AddUserAsync(auth);
            var actorId = await _permissionService.GetUserId();
            if (succes)
            {
                _logger.LogInformation("Użytkownik {ActorId} dodał użytkownika {NewUserId} (rola {RoleId})", actorId, auth.Id, roleId.Value);
                _notificationService.Show("Dodawane użytkownika powiodło się!", EnumNotificationType.Success);
            }
            else
            {
                _logger.LogWarning("Użytkownik {ActorId} nie zdołał dodać użytkownika (błąd lub e-mail już istnieje)", actorId);
                _notificationService.Show("Dodawane użytkownika nie powiodło się lub email juz istnieje!", EnumNotificationType.Error);
            }
            return succes;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.ManageUser);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa zmiany roli użytkownika {UserId}: brak uprawnienia {Permission}", userId, EnumPermissions.ManageUser);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            if (await _roleRepository.GetRoleByIdAsync(roleId) == null)
            {
                _logger.LogWarning("Zmiana roli użytkownika {UserId} odrzucona: rola {RoleId} nie istnieje", userId, roleId);
                _notificationService.Show("Wybrana rola nie istnieje!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _authRepository.UpdateUserRoleAsync(userId, roleId);
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} zmienił rolę użytkownika {UserId} na {RoleId}", actorId, userId, roleId);
                _notificationService.Show("Zmieniono stanowisko użytkownika!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Zmiana stanowiska nie powiodła się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.ManageUser);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa usunięcia użytkownika {UserId}: brak uprawnienia {Permission}", userId, EnumPermissions.ManageUser);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            var actorId = await _permissionService.GetUserId();
            if (userId == actorId)
            {
                _logger.LogWarning("Użytkownik {ActorId} próbował usunąć własne konto", actorId);
                _notificationService.Show("Nie możesz usunąć własnego konta!", EnumNotificationType.Failure);
                return false;
            }

            var targetPermissions = await _permissionService.GetUserPermissionsAsync(userId);
            if (targetPermissions.Contains(EnumPermissions.Master))
            {
                _logger.LogWarning("Użytkownik {ActorId} próbował usunąć konto administratora {UserId}", actorId, userId);
                _notificationService.Show("Nie można usunąć konta administratora!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _authRepository.DeleteUserAsync(userId);
            if (success)
            {
                _logger.LogInformation("Użytkownik {ActorId} usunął użytkownika {UserId}", actorId, userId);
                _notificationService.Show("Użytkownik został usunięty!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Usunięcie użytkownika nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<bool> MustChangePasswordAsync(int userId)
        {
            if (userId == 0)
            {
                return false;
            }
            var user = await _authRepository.GetUserByIdAsync(userId);
            return user?.MustChangePassword ?? false;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = await _permissionService.GetUserId();
            if (userId == 0)
            {
                _notificationService.Show("Musisz być zalogowany!", EnumNotificationType.Failure);
                return false;
            }
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                _notificationService.Show("Nie znaleziono konta!", EnumNotificationType.Failure);
                return false;
            }
            // Hasło można zmienić tylko raz: przy pierwszym logowaniu (hasło nadał administrator).
            if (!user.MustChangePassword)
            {
                _logger.LogWarning("Odrzucono zmianę hasła użytkownika {UserId}: to nie pierwsze logowanie", userId);
                _notificationService.Show("Hasło można zmienić tylko przy pierwszym logowaniu!", EnumNotificationType.Failure);
                return false;
            }
            if (string.IsNullOrEmpty(currentPassword))
            {
                _notificationService.Show("Podaj aktualne hasło!", EnumNotificationType.Failure);
                return false;
            }
            if (newPassword != confirmPassword)
            {
                _notificationService.Show("Nowe hasła nie są takie same!", EnumNotificationType.Failure);
                return false;
            }
            if (newPassword.Length < 8 || newPassword.Length > 128
                || !newPassword.Any(char.IsLetter) || !newPassword.Any(char.IsDigit))
            {
                _notificationService.Show("Hasło musi mieć min. 8 znaków, w tym literę i cyfrę!", EnumNotificationType.Failure);
                return false;
            }
            if (newPassword == currentPassword)
            {
                _notificationService.Show("Nowe hasło musi się różnić od obecnego!", EnumNotificationType.Failure);
                return false;
            }

            var hasher = new PasswordHasher<string>();
            var verification = hasher.VerifyHashedPassword(user.Email, user.Password, currentPassword);
            if (verification == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Zmiana hasła odrzucona: błędne aktualne hasło użytkownika {UserId}", userId);
                _notificationService.Show("Aktualne hasło jest nieprawidłowe!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _authRepository.UpdatePasswordAsync(userId, hasher.HashPassword(user.Email, newPassword), false);
            if (success)
            {
                _notificationService.Show("Hasło zostało zmienione!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Zmiana hasła nie powiodła się!", EnumNotificationType.Error);
            }
            return success;
        }

        public async Task<int> CheckIfRegisteredAsync(string login, string password)
        {
            var hasher = new PasswordHasher<string>();
            try
            {
                var user = await _authRepository.CheckIfRegisteredAsync(login);
                var result = hasher.VerifyHashedPassword(login, user.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    _logger.LogInformation("Zalogowano użytkownika {UserId}", user.Id);
                    return user.Id;
                }

                _logger.LogWarning("Nieudane logowanie: błędne hasło użytkownika {UserId}", user.Id);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Nieudane logowanie: {Reason}", ex.Message);
                return 0;
            }
        }
    }
}
