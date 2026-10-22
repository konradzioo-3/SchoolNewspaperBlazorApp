namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IAuthService
    {
        Task AddUserAsync(string password, string email);
        Task<int> CheckIfRegisteredAsync(string login, string password);
        Task<Dictionary<int, string>> GetAllUsersAsync();
    }
}
