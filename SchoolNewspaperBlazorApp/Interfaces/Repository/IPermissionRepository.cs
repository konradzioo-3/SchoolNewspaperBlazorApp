using SchoolNewspaperBlazorApp.Data;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IPermissionRepository
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal principal, EnumPermission permissions);
        Task SavePermitionsAsync(List<EnumPermission> permissions, int userId);
    }
}
