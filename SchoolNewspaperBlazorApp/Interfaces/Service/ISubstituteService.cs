using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface ISubstituteService
    {
        Task<bool> AddSubstituteAsync(
            string className,
            int? originalTeacherId,
            int? substituteTeacherId,
            string? subject,
            int? lessonNumber,
            DateTime? originalDate,
            DateTime? substituteDate,
            string? oldRoom,
            string? room,
            string? note,
            IReadOnlyDictionary<int, string?>? customValues = null);
        Task<bool> RemoveSubstituteAsync(int id);
        Task<List<SubstituteLessons>> GetUpcomingSubstitutesAsync();
        Task<List<SubstituteLessons>> GetRecentSubstitutesAsync(int count = 50);
    }
}
