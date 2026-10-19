namespace SchoolNewspaperBlazorApp.Data
{
    public class UserPermision
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public User User { get; set; } = null!;
        public Permission Permission { get; set; }
    }
}
