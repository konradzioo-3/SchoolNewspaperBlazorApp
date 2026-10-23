
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Data.Seeders
{
    public static class PermissionService
    {
        public static async Task SeedAsync(
            IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            await using var context =
                await contextFactory.CreateDbContextAsync();

            var permissions = new Dictionary<EnumPermissions, string>
            {
                { EnumPermissions.AddArticle, "Dodawanie artykułów" },
                { EnumPermissions.EditArticle, "Edycja artykułów" },
                { EnumPermissions.DeleteArticle, "Usuwanie artykułów" },
                { EnumPermissions.AddUser, "Dodawanie użytkowników" },
                { EnumPermissions.ManageUser, "Zarządzanie użytkownikami" },
                { EnumPermissions.AddFile, "Dodawanie plików" },
                { EnumPermissions.Master, "Master" }
            };

            foreach (var permission in permissions)
            {
                bool exists = await context.Permissions
                    .AnyAsync(p => p.PermissionName == permission.Key);

                if (!exists)
                {
                    context.Permissions.Add(new Permission
                    {
                        PermissionName = permission.Key,
                        PermissionViewName = permission.Value
                    });
                }
            }

            await context.SaveChangesAsync();

            const string adminEmail = "admin@admin.pl";
            const string adminPassword = "admin";

            var admin = await context.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (admin == null)
            {
                var hasher = new PasswordHasher<string>();

                string passwordHash = hasher.HashPassword(
                    null!,
                    adminPassword);

                admin = new User
                {
                    Email = adminEmail,
                    Password = passwordHash,
                    RegisterDate = DateTime.Now,
                    Name= adminEmail,
                    Surname= adminEmail,
                    Role= "Admin",
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }

            int masterPermissionId = await context.Permissions
                .Where(p => p.PermissionName == EnumPermissions.Master)
                .Select(p => p.Id)
                .SingleAsync();

            bool masterExists = await context.UserPermissions
                .AnyAsync(up =>
                    up.UserId == admin.Id &&
                    up.PermissionId == masterPermissionId);

            if (!masterExists)
            {
                context.UserPermissions.Add(new UserPermission
                {
                    UserId = admin.Id,
                    PermissionId = masterPermissionId,
                    PermissionName = "Administrator"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
