using Microsoft.AspNetCore.Authentication;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using SchoolNewspaperBlazorApp.Repository;


using Microsoft.AspNetCore.Identity;
namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task AddUserAsync(string login, string password, string email)
        {
            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);
            var auth = new User
            {
                Login = login,
                Password = passwordHash,
                Email = email,
                RegisterDate = DateTime.Now
            };
            await _authRepository.AddUserAsync(auth);
        }

        public async Task<bool> CheckIfRegisteredAsync(string login, string password)
        {
            var haser = new PasswordHasher<string>();
            
            try
            {
                var dbPass = await _authRepository.CheckIfRegisteredAsync(login);
                var result = haser.VerifyHashedPassword(login, dbPass, password);
                if (result == PasswordVerificationResult.Success)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

       

        
        }

    }
}
