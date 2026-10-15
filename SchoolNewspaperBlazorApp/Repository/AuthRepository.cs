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

        public async Task AddUserAsync(User user)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> CheckIfRegisteredAsync(string login)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login);
               
            if (user != null)
            {
                return user.Password;
            }
            else
            {
                throw new Exception("Konto nie istnieje");
            }
        }
    }
}
    
