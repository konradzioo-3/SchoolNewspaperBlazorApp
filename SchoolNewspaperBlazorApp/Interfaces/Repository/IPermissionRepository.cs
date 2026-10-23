using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IPermissionRepository
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal principal, EnumPermissions permission);
        Task<bool> SavePermissionsAsync(List<EnumPermissions> permissions, int userId);
        Task<List<EnumPermissions>> GetUserPermissionsAsync(int userId);
        Task<Dictionary<EnumPermissions, bool>> GetAllUserPermissionsAsync(int userId);
    }
}
