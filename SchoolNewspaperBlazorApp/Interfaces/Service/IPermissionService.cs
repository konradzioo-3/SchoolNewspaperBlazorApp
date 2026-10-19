using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{ 
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(EnumPermission permissions);
        Task SavePermissionsAsync(List<EnumPermission> permissions, int userId);
    }
}
