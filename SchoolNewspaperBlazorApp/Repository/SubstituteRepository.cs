using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class SubstituteRepository : ISubstituteRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<SubstituteRepository> _logger;

        public SubstituteRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<SubstituteRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<bool> AddSubstituteAsync(SubstituteLessons substitute)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                await context.Substitutes.AddAsync(substitute);
                await context.SaveChangesAsync();
                _logger.LogInformation("Dodano zastępstwo {SubstituteId}", substitute.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania zastępstwa");
                return false;
            }
        }

        // Soft delete: wiersz zostaje w bazie z IsDeleted = 1.
        public async Task<bool> RemoveSubstituteAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var substitute = await context.Substitutes.FirstOrDefaultAsync(s => s.Id == id);
                if (substitute == null)
                {
                    _logger.LogWarning("Usunięcie zastępstwa nie powiodło się: zastępstwo {SubstituteId} nie istnieje", id);
                    return false;
                }
                substitute.IsDeleted = true;
                await context.SaveChangesAsync();
                _logger.LogInformation("Usunięto (soft delete) zastępstwo {SubstituteId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania zastępstwa {SubstituteId}", id);
                return false;
            }
        }

        public async Task<List<SubstituteLessons>> GetUpcomingSubstitutesAsync(DateTime fromDate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Substitutes
                .AsNoTracking()
                .Include(s => s.OriginalTeacher)
                .Include(s => s.SubstituteTeacher)
                .Include(s => s.FieldValues)
                .Where(s => s.SubstituteDate >= fromDate.Date)
                .OrderBy(s => s.SubstituteDate)
                .ThenBy(s => s.LessonNumber)
                .ThenBy(s => s.ClassName)
                .ToListAsync();
        }

        public async Task<List<SubstituteLessons>> GetRecentSubstitutesAsync(int count)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Substitutes
                .AsNoTracking()
                .Include(s => s.OriginalTeacher)
                .Include(s => s.SubstituteTeacher)
                .Include(s => s.FieldValues)
                .OrderByDescending(s => s.SubstituteDate)
                .ThenBy(s => s.LessonNumber)
                .ThenBy(s => s.ClassName)
                .Take(count)
                .ToListAsync();
        }
    }
}
