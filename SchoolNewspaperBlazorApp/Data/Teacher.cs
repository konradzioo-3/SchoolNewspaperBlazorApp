namespace SchoolNewspaperBlazorApp.Data
{
    public class Teacher
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        // Zarchiwizowany nauczyciel znika z list wyboru, ale zostaje w starych zastępstwach.
        public bool IsArchived { get; set; }
        public string FullName => $"{Surname} {Name}";
    }
}
