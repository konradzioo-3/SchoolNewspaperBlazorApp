using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<bool> AddRoleAsync(string name);
    }
}
