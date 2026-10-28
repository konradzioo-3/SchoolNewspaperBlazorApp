using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface ISchoolDictionaryRepository
    {
        Task<List<DictionaryItem>> GetAllAsync(EnumDictionaryKind kind);
        Task<bool> ExistsAsync(EnumDictionaryKind kind, string name);
        // Dodaje wpis; gdy wcześniej usunięty wpis o tej nazwie istnieje - przywraca go.
        Task<bool> AddAsync(EnumDictionaryKind kind, string name);
        Task<bool> RemoveAsync(EnumDictionaryKind kind, int id);
    }
}
