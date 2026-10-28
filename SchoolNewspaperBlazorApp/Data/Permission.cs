using SchoolNewspaperBlazorApp.Data.Enums;
using System.Runtime.Serialization;

namespace SchoolNewspaperBlazorApp.Data
{
    public class Permission
    {
        public int Id { get; set; }
        public EnumPermissions PermissionName { get; set; }
        public string PermissionViewName { get; set; } = string.Empty;
        public ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
    }
}
