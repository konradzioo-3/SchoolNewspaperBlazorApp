using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface ISchoolDictionaryService
    {
        Task<List<DictionaryItem>> GetAllAsync(EnumDictionaryKind kind);
        Task<bool> AddAsync(EnumDictionaryKind kind, string name);
        Task<bool> RemoveAsync(EnumDictionaryKind kind, int id);
    }
}
