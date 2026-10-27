using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IAuthService
    {
        Task<bool> AddUserAsync(string password, string email, string name, string surname, int? roleId);
        Task<int> CheckIfRegisteredAsync(string login, string password);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> MustChangePasswordAsync(int userId);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword);
    }
}
