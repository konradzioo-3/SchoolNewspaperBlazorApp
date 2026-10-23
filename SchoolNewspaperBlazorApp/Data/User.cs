namespace SchoolNewspaperBlazorApp.Data
{
    public class User
    {    
        public int Id { get; set;}
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Role { get; set; }
        public required DateTime RegisterDate { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
