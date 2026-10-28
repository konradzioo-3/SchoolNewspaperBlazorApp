using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface ICustomFieldRepository
    {
        Task<List<CustomField>> GetActiveAsync();
        Task<bool> AddAsync(CustomField field);
        Task<bool> RemoveAsync(int id);
    }
}
