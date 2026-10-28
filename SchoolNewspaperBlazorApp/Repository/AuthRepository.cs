using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(IDbContextFactory<NewspaperDbContext> contextFactory, ILogger<AuthRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika {UserId}", userId);
            }
            return user!;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.Surname)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("Zmiana roli nie powiodła się: użytkownik {UserId} nie istnieje", userId);
                    return false;
                }

                user.RoleId = roleId;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Zmieniono rolę użytkownika {UserId} na {RoleId}", userId, roleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany roli użytkownika {UserId}", userId);
                return false;
            }
        }

        // Soft delete: konto zostaje w bazie z IsDeleted = 1 (nie można się na nie zalogować,
        // a jego uprawnienia przestają działać dzięki filtrom zapytań).
        public async Task<bool> DeleteUserAsync(int userId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("Usunięcie użytkownika nie powiodło się: użytkownik {UserId} nie istnieje", userId);
                    return false;
                }

                user.IsDeleted = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Usunięto (soft delete) użytkownika {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania użytkownika {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash, bool mustChangePassword)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("Zmiana hasła nie powiodła się: użytkownik {UserId} nie istnieje", userId);
                    return false;
                }

                user.Password = passwordHash;
                user.MustChangePassword = mustChangePassword;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Zmieniono hasło użytkownika {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany hasła użytkownika {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AddUserAsync(User user)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var isAlreadyAdded = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (isAlreadyAdded)
                {
                    _logger.LogWarning("Dodanie użytkownika odrzucone: e-mail już istnieje w bazie");
                    return false;
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Dodano użytkownika {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania użytkownika");
                return false;
            }
        }

        public async Task<User> CheckIfRegisteredAsync(string login)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login);
            if (user != null)
            {
                return user;
            }

            // Celowo nie logujemy wpisanego loginu - bywa, że ktoś wpisze w to pole hasło.
            _logger.LogInformation("Próba logowania na nieistniejące konto");
            throw new Exception("Konto nie istnieje");
        }
    }
}
