namespace SchoolNewspaperBlazorApp.Data
{
    public class SubstituteLessons
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = "";
        public int? OriginalTeacherId { get; set; }
        public Teacher? OriginalTeacher { get; set; }
        public int? SubstituteTeacherId { get; set; }
        public Teacher? SubstituteTeacher { get; set; }
        public string? Subject { get; set; }
        public int? LessonNumber { get; set; }
        public DateTime OriginalDate { get; set; }
        public DateTime SubstituteDate { get; set; }
        public string? OldRoom { get; set; }
        public string? Room { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        // Soft delete: false = 0 (aktywne), true = 1 (usunięte). Usunięte rekordy zostają w bazie.
        public bool IsDeleted { get; set; }
        public ICollection<SubstituteFieldValue> FieldValues { get; set; } = new List<SubstituteFieldValue>();
    }
}
