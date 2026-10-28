using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface ITeacherService
    {
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> AddTeacherAsync(string name, string surname);
        Task<bool> RemoveTeacherAsync(int id);
    }
}
