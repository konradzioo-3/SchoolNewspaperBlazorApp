namespace SchoolNewspaperBlazorApp.Data
{
    public class User
    {    
        public int Id { get; set;}
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required DateTime RegisterDate { get; set; }
        public string AddBy {  get; set; } = string.Empty;
        public ICollection<UserPermision> UserPermisions { get; set; } = new List<UserPermision>();
    }
}
