using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task<bool> AddUserAsync(User user);
        Task<User> CheckIfRegisteredAsync(string login);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash, bool mustChangePassword);
    }
}
