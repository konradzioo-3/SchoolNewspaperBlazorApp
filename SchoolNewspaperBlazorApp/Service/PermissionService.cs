using Microsoft.AspNetCore.Components.Authorization;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly INotificationService _notificationService;
        private readonly AuthenticationStateProvider _asp;
        public PermissionService(IPermissionRepository permissionRepository, AuthenticationStateProvider asp, INotificationService notificationService  )
        {
            _permissionRepository = permissionRepository;
            _asp = asp;
            _notificationService = notificationService;
        }

        public async Task<List<EnumPermissions>> GetUserPermissionsAsync(int userId)
        {
            return await _permissionRepository.GetUserPermissionsAsync(userId);
        }
        public async Task<Dictionary<EnumPermissions,bool>> GetAllPermissionsAsync()
        {
            var id = await GetUserId();
            return await _permissionRepository.GetAllUserPermissionsAsync(id);
        }
        public async Task<bool> HasPermissionAsync(EnumPermissions permission)
        {
            var authState = await _asp.GetAuthenticationStateAsync();
            var principal = authState.User;
            return await _permissionRepository.HasPermissionAsync(principal, permission);
        }
        public async Task SavePermissionsAsync(List<EnumPermissions> permissions, int userId)
        {
            bool hasPermission = await HasPermissionAsync(EnumPermissions.Master);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return;
            }
            var succces = await _permissionRepository.SavePermissionsAsync(permissions, userId);
            if(succces)
            {
                _notificationService.Show("Nadano uprawnienia!", EnumNotificationType.Success);

            }
            else
            {
                _notificationService.Show("Bład poczasz nadawania uprawnień! Skontaktuj się z obsługa techniczną!", EnumNotificationType.Failure);

            }
        }
        public async Task<int> GetUserId()
        {
            var authState = await _asp.GetAuthenticationStateAsync();
            var principal = authState.User;
            var stringId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(stringId, out int id))
            {
                return id;
            }

            return 0;
        }

    }
}
