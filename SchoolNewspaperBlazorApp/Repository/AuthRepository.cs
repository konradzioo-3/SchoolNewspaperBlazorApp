using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        public AuthRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.Users
                .ToDictionaryAsync(u => u.Id, u => u.Email);
        }
        public async Task<bool> AddUserAsync(User user)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var isAlreadyAdded = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (isAlreadyAdded)
                {
                    return false;
                }

                else
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
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
            else
            {
                throw new Exception("Konto nie istnieje");
            }
        }
    }
}

