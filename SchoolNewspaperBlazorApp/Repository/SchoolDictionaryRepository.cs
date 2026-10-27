using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class SchoolDictionaryRepository : ISchoolDictionaryRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<SchoolDictionaryRepository> _logger;

        public SchoolDictionaryRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<SchoolDictionaryRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public Task<List<DictionaryItem>> GetAllAsync(EnumDictionaryKind kind) => kind switch
        {
            EnumDictionaryKind.Class => GetAllCoreAsync<SchoolClass>(),
            EnumDictionaryKind.Room => GetAllCoreAsync<Room>(),
            EnumDictionaryKind.Subject => GetAllCoreAsync<Subject>(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        public Task<bool> ExistsAsync(EnumDictionaryKind kind, string name) => kind switch
        {
            EnumDictionaryKind.Class => ExistsCoreAsync<SchoolClass>(name),
            EnumDictionaryKind.Room => ExistsCoreAsync<Room>(name),
            EnumDictionaryKind.Subject => ExistsCoreAsync<Subject>(name),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        public Task<bool> AddAsync(EnumDictionaryKind kind, string name) => kind switch
        {
            EnumDictionaryKind.Class => AddCoreAsync<SchoolClass>(name),
            EnumDictionaryKind.Room => AddCoreAsync<Room>(name),
            EnumDictionaryKind.Subject => AddCoreAsync<Subject>(name),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        public Task<bool> RemoveAsync(EnumDictionaryKind kind, int id) => kind switch
        {
            EnumDictionaryKind.Class => RemoveCoreAsync<SchoolClass>(id),
            EnumDictionaryKind.Room => RemoveCoreAsync<Room>(id),
            EnumDictionaryKind.Subject => RemoveCoreAsync<Subject>(id),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        private async Task<List<DictionaryItem>> GetAllCoreAsync<T>() where T : DictionaryItem
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var items = await context.Set<T>().AsNoTracking().OrderBy(x => x.Name).ToListAsync();
            return items.Cast<DictionaryItem>().ToList();
        }

        private async Task<bool> ExistsCoreAsync<T>(string name) where T : DictionaryItem
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<T>().AnyAsync(x => x.Name == name);
        }

        private async Task<bool> AddCoreAsync<T>(string name) where T : DictionaryItem, new()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                // Nazwa jest unikalna także wśród usuniętych wpisów - w takim wypadku przywracamy stary wpis.
                var existing = await context.Set<T>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Name == name);
                if (existing != null)
                {
                    if (!existing.IsDeleted)
                    {
                        return false;
                    }
                    existing.IsDeleted = false;
                }
                else
                {
                    await context.Set<T>().AddAsync(new T { Name = name });
                }
                await context.SaveChangesAsync();
                _logger.LogInformation("Dodano wpis słownika {Type}", typeof(T).Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania wpisu słownika {Type}", typeof(T).Name);
                return false;
            }
        }

        // Soft delete: wiersz zostaje w bazie z IsDeleted = 1.
        private async Task<bool> RemoveCoreAsync<T>(int id) where T : DictionaryItem
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var item = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    _logger.LogWarning("Usunięcie wpisu {Type} {Id} nie powiodło się: wpis nie istnieje", typeof(T).Name, id);
                    return false;
                }
                item.IsDeleted = true;
                await context.SaveChangesAsync();
                _logger.LogInformation("Usunięto (soft delete) wpis {Type} {Id}", typeof(T).Name, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania wpisu {Type} {Id}", typeof(T).Name, id);
                return false;
            }
        }
    }
}
