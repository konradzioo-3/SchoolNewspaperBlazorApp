namespace SchoolNewspaperBlazorApp.Data
{
    public class User
    {    
        public int Id { get; set;}
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public required DateTime RegisterDate { get; set; }
        // true = użytkownik musi ustawić własne hasło przy pierwszym logowaniu.
        public bool MustChangePassword { get; set; }
        // Soft delete: false = 0 (aktywny), true = 1 (usunięty). Usunięte rekordy zostają w bazie.
        public bool IsDeleted { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
