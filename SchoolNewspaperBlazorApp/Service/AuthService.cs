using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using SchoolNewspaperBlazorApp.Repository;
using System.Text.RegularExpressions;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private static readonly Regex EmailRegex = new(
    @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"
);
        public AuthService(IAuthRepository authRepository, IPermissionService permissionService, INotificationService notificationService   )
        {
            _authRepository = authRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }
        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }
        public async Task AddUserAsync(string password, string email)
        {
            if(string.IsNullOrWhiteSpace(password))
            {
                _notificationService.Show("Hasło jest puste!", EnumNotificationType.Failure);
                return;
            }
            if(!EmailRegex.IsMatch(email))
            {
                _notificationService.Show("E-mail jest niepoprawny!", EnumNotificationType.Failure);
                return;

            }
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddUser);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return;
            }


            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);
            var auth = new User
            {
                Password = passwordHash,
                Email = email,
                RegisterDate = DateTime.Now
            };
            var succes = await _authRepository.AddUserAsync(auth);
            if(succes)
            {
                _notificationService.Show("Dodawane użytkownika powiodło się!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show("Dodawane użytkownika nie powiodło się lub email juz istnieje!", EnumNotificationType.Error);
            }
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
                    return user.Id;
                }
                else
                {

                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }

        }


    }
}
