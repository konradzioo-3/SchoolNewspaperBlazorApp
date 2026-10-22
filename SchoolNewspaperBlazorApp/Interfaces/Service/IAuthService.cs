using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IAuthService
    {
        Task<bool> AddUserAsync(string password, string email,string name,string surname);
        Task<int> CheckIfRegisteredAsync(string login, string password);
        Task<List<User>> GetAllUsersAsync();
    }
}
