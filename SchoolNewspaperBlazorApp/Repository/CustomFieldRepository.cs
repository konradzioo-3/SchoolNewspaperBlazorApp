using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class CustomFieldRepository : ICustomFieldRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<CustomFieldRepository> _logger;

        public CustomFieldRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<CustomFieldRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<CustomField>> GetActiveAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.CustomFields
                .AsNoTracking()
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Id)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(CustomField field)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var lastOrder = await context.CustomFields.MaxAsync(f => (int?)f.SortOrder) ?? 0;
                field.SortOrder = lastOrder + 1;
                await context.CustomFields.AddAsync(field);
                await context.SaveChangesAsync();
                _logger.LogInformation("Dodano własne pole formularza {FieldId}", field.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania własnego pola formularza");
                return false;
            }
        }

        // Soft delete: wartości wpisane w starych zastępstwach zostają w bazie.
        public async Task<bool> RemoveAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var field = await context.CustomFields.FirstOrDefaultAsync(f => f.Id == id);
                if (field == null)
                {
                    return false;
                }
                field.IsDeleted = true;
                await context.SaveChangesAsync();
                _logger.LogInformation("Usunięto (soft delete) własne pole formularza {FieldId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania własnego pola formularza {FieldId}", id);
                return false;
            }
        }
    }
}
