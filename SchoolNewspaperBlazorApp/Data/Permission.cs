
namespace SchoolNewspaperBlazorApp.Data
{
    public class Permission
    {
        public int Id { get; set; }
        public EnumPermission PermissionName { get; set; }

        public ICollection<UserPermision> Permissions { get; set; } = new List<UserPermision>();
    }
}
