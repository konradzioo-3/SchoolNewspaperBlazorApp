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
        public DbSet<MediaFile> Files { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserPermision> UserPermisions { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<MediaFile>()
                .Property(x => x.Id)
                .ValueGeneratedNever();
            mb.Entity<UserPermision>()
                .HasKey(up => new { up.UserId, up.PermissionId });
            mb.Entity<UserPermision>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermisions)
                .HasForeignKey(u => u.UserId);
            mb.Entity<UserPermision>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.Permissions)
                .HasForeignKey(up => up.PermissionId);
        }
    }

}
