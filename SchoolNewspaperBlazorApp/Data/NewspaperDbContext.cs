using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace SchoolNewspaperBlazorApp.Data
{
    public class NewspaperDbContext : DbContext
    {
        public NewspaperDbContext(DbContextOptions<NewspaperDbContext> options)
            : base(options) {}
        public DbSet<Article> Articles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MediaFile> Files { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<SubstituteLessons> Substitutes { get; set; }
        public DbSet<ArticleBlock> ArticleBlocks { get; set; }
        public DbSet<SchoolClass> Classes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<SubstituteFieldValue> SubstituteFieldValues { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<MediaFile>()
                .Property(x => x.Id)
                .ValueGeneratedNever();
            // Soft delete: rekordy z IsDeleted = 1 są automatycznie pomijane we wszystkich zapytaniach.
            // Żeby zobaczyć także usunięte, użyj .IgnoreQueryFilters().
            mb.Entity<Article>().HasQueryFilter(a => !a.IsDeleted);
            mb.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            mb.Entity<SubstituteLessons>().HasQueryFilter(s => !s.IsDeleted);
            mb.Entity<ArticleBlock>().HasQueryFilter(b => !b.Article.IsDeleted);
            mb.Entity<SchoolClass>().HasQueryFilter(x => !x.IsDeleted);
            mb.Entity<Room>().HasQueryFilter(x => !x.IsDeleted);
            mb.Entity<Subject>().HasQueryFilter(x => !x.IsDeleted);
            mb.Entity<CustomField>().HasQueryFilter(x => !x.IsDeleted);
            // Wartość własnego pola znika razem z usuniętym zastępstwem lub polem.
            mb.Entity<SubstituteFieldValue>().HasQueryFilter(v => !v.CustomField.IsDeleted && !v.SubstituteLesson.IsDeleted);
            // UserPermission wymaga User - filtr musi być spójny, żeby uprawnienia
            // usuniętego użytkownika nie były nigdzie widoczne.
            mb.Entity<UserPermission>().HasQueryFilter(up => !up.User.IsDeleted);
            mb.Entity<Role>(e =>
            {
                e.Property(r => r.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(r => r.Name).IsUnique();
            });
            // Rola, do której przypisani są użytkownicy, nie może zniknąć "po cichu".
            mb.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            mb.Entity<Article>()
                .Property(a => a.AuthorRole)
                .HasMaxLength(100);
            mb.Entity<Teacher>(e =>
            {
                e.Property(t => t.Name).IsRequired().HasMaxLength(100);
                e.Property(t => t.Surname).IsRequired().HasMaxLength(100);
                e.Ignore(t => t.FullName);
                e.HasIndex(t => new { t.Surname, t.Name }).IsUnique();
            });
            mb.Entity<SubstituteLessons>(e =>
            {
                e.ToTable("SubstituteLessons");
                e.Property(x => x.ClassName).IsRequired().HasMaxLength(50);
                e.Property(x => x.Subject).HasMaxLength(100);
                e.Property(x => x.OldRoom).HasMaxLength(50);
                e.Property(x => x.Room).HasMaxLength(50);
                e.Property(x => x.Note).HasMaxLength(500);
                // Nauczyciel, który ma przypisane zastępstwa, nie może zniknąć "po cichu".
                e.HasOne(x => x.OriginalTeacher)
                    .WithMany()
                    .HasForeignKey(x => x.OriginalTeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.SubstituteTeacher)
                    .WithMany()
                    .HasForeignKey(x => x.SubstituteTeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => x.SubstituteDate);
            });
            mb.Entity<ArticleBlock>(e =>
            {
                e.HasOne(b => b.Article)
                    .WithMany()
                    .HasForeignKey(b => b.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(b => new { b.ArticleId, b.Position });
            });
            mb.Entity<SchoolClass>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
                e.HasIndex(x => x.Name).IsUnique();
            });
            mb.Entity<Room>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
                e.HasIndex(x => x.Name).IsUnique();
            });
            mb.Entity<Subject>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique();
            });
            mb.Entity<CustomField>(e =>
            {
                e.Property(x => x.Label).IsRequired().HasMaxLength(100);
                e.Property(x => x.Options).HasMaxLength(1000);
            });
            mb.Entity<SubstituteFieldValue>(e =>
            {
                e.Property(x => x.Value).IsRequired().HasMaxLength(500);
                e.HasOne(x => x.SubstituteLesson)
                    .WithMany(s => s.FieldValues)
                    .HasForeignKey(x => x.SubstituteLessonId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.CustomField)
                    .WithMany()
                    .HasForeignKey(x => x.CustomFieldId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            mb.Entity<UserPermission>()
                .HasKey(up => new { up.UserId, up.PermissionId });
            mb.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(u => u.UserId);
            mb.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.Permissions)
                .HasForeignKey(up => up.PermissionId);
        }
    }

}
