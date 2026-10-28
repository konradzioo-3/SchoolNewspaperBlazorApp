using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    // Role można tylko dodawać i odczytywać. Edycja i usuwanie ról
    // jest celowo pominięta - robi się to wyłącznie w SQL.
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<RoleRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> RoleExistsAsync(string name)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Roles.AnyAsync(r => r.Name == name);
        }

        public async Task<bool> AddRoleAsync(Role role)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Dodano rolę {RoleId}", role.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania roli");
                return false;
            }
        }
    }
}
