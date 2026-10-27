using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface ICustomFieldService
    {
        Task<List<CustomField>> GetActiveAsync();
        Task<bool> AddAsync(string label, EnumCustomFieldType type, string? options, bool isRequired, bool showOnList);
        Task<bool> RemoveAsync(int id);
    }
}
