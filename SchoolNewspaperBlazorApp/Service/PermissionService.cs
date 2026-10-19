using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Internal;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly AuthenticationStateProvider _asp;
        public PermissionService(IPermissionRepository permissionRepository, AuthenticationStateProvider asp)
        {
            _permissionRepository = permissionRepository;
            _asp = asp;
        }

        public async Task<bool> HasPermissionAsync( EnumPermission permissions)
        {
            var authState = await _asp.GetAuthenticationStateAsync();
            var principal = authState.User;
            return await _permissionRepository.HasPermissionAsync(principal, permissions);

        }
      public async Task SavePermissionsAsync(List<EnumPermission> permissions, int userId)
        {
            await _permissionRepository.SavePermitionsAsync(permissions, userId);
        }


    }
}

