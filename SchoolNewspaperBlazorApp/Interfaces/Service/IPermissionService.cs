using SchoolNewspaperBlazorApp.Data.Enums;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(EnumPermissions permission);
        Task<bool> SavePermissionsAsync(List<EnumPermissions> permissions, int userId);
        Task<List<EnumPermissions>> GetUserPermissionsAsync(int userId);
        Task<Dictionary<EnumPermissions, bool>> GetAllPermissionsAsync();
        Task<int> GetUserId();
    }
}
