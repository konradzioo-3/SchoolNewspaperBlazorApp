namespace SchoolNewspaperBlazorApp.Data
{
    public class SubstituteLessons
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;
        public string ClassName { get; set; } = "";
        public string? OriginalTeacher { get; set; }
        public string? SubstituteTeacher { get; set; }
        public string? Subject { get; set; }
        public DateTime OrginalDate { get; set; }
        public DateTime SubstituteDate { get; set; }
        public string? OldRoom { get; set; }
        public string? Room { get; set; }
    }
}
