using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Text.RegularExpressions;

namespace SchoolNewspaperBlazorApp.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, IPermissionService permissionService, INotificationService notificationService, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<bool> AddRoleAsync(string name)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.ManageUser);
            if (!hasPermission)
            {
                _logger.LogWarning("Odmowa dodania roli: brak uprawnienia {Permission}", EnumPermissions.ManageUser);
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            // przytnij i zamień ciągi białych znaków (w tym entery) na pojedynczą spację
            name = Regex.Replace((name ?? string.Empty).Trim(), @"\s+", " ");
            if (name.Length == 0)
            {
                _notificationService.Show("Nazwa roli nie może być pusta!", EnumNotificationType.Failure);
                return false;
            }
            if (name.Length > 100)
            {
                _notificationService.Show("Nazwa roli jest za długa (max 100 znaków)!", EnumNotificationType.Failure);
                return false;
            }
            if (await _roleRepository.RoleExistsAsync(name))
            {
                _notificationService.Show("Taka rola już istnieje!", EnumNotificationType.Failure);
                return false;
            }

            var success = await _roleRepository.AddRoleAsync(new Role { Name = name });
            if (success)
            {
                var actorId = await _permissionService.GetUserId();
                _logger.LogInformation("Użytkownik {ActorId} dodał rolę \"{RoleName}\"", actorId, name);
                _notificationService.Show("Dodano rolę!", EnumNotificationType.Success);
            }
            else
            {
                _logger.LogError("Nie udało się dodać roli \"{RoleName}\"", name);
                _notificationService.Show("Dodanie roli nie powiodło się!", EnumNotificationType.Error);
            }
            return success;
        }
    }
}
