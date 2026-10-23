using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task<bool> AddUserAsync(User user);
        Task<User> CheckIfRegisteredAsync(string login);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
    }
}
