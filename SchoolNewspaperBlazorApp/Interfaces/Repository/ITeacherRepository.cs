using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface ITeacherRepository
    {
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<bool> TeacherExistsAsync(string name, string surname);
        Task<Teacher?> AddTeacherAsync(Teacher teacher);
        Task<bool> ArchiveTeacherAsync(int id);
    }
}
