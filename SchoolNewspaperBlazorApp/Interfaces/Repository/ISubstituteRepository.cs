using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface ISubstituteRepository
    {
        Task<bool> AddSubstituteAsync(SubstituteLessons substitute);
        Task<bool> RemoveSubstituteAsync(int id);
        // Zastępstwa, których data zastępstwa jest >= fromDate (od najbliższych).
        Task<List<SubstituteLessons>> GetUpcomingSubstitutesAsync(DateTime fromDate);
        Task<List<SubstituteLessons>> GetRecentSubstitutesAsync(int count);
    }
}
