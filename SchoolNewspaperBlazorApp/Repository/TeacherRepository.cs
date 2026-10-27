using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<TeacherRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Teachers
                .Where(t => !t.IsArchived)
                .OrderBy(t => t.Surname)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<bool> TeacherExistsAsync(string name, string surname)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Teachers.AnyAsync(t => t.Name == name && t.Surname == surname && !t.IsArchived);
        }

        public async Task<Teacher?> AddTeacherAsync(Teacher teacher)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                // Para (nazwisko, imię) jest unikalna także wśród zarchiwizowanych - wtedy przywracamy starego nauczyciela.
                var archived = await context.Teachers
                    .FirstOrDefaultAsync(t => t.Name == teacher.Name && t.Surname == teacher.Surname && t.IsArchived);
                if (archived != null)
                {
                    archived.IsArchived = false;
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Przywrócono nauczyciela {TeacherId}", archived.Id);
                    return archived;
                }

                await context.Teachers.AddAsync(teacher);
                await context.SaveChangesAsync();
                _logger.LogInformation("Dodano nauczyciela {TeacherId}", teacher.Id);
                return teacher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania nauczyciela");
                return null;
            }
        }

        // Archiwizacja zamiast usuwania: stare zastępstwa nadal pokazują nazwisko nauczyciela.
        public async Task<bool> ArchiveTeacherAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Id == id && !t.IsArchived);
                if (teacher == null)
                {
                    return false;
                }
                teacher.IsArchived = true;
                await context.SaveChangesAsync();
                _logger.LogInformation("Zarchiwizowano nauczyciela {TeacherId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas archiwizacji nauczyciela {TeacherId}", id);
                return false;
            }
        }
    }
}
