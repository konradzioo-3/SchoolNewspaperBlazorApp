using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task AddUserAsync(User user);
        Task<User> CheckIfRegisteredAsync(string login);

        Task<Dictionary<int,string>> GetAllUsersAsync();

    }
}
