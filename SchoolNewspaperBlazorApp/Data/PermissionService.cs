
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
                { EnumPermissions.Master, "Master" },
                { EnumPermissions.AddSubstituteLesson, "Dodawanie zastępstw" },
                { EnumPermissions.ManageClasses, "Zarządzanie klasami" },
                { EnumPermissions.ManageTeachers, "Zarządzanie nauczycielami" },
                { EnumPermissions.ManageRooms, "Zarządzanie salami" },
                { EnumPermissions.ManageSubjects, "Zarządzanie przedmiotami" },
                { EnumPermissions.ManageCustomFields, "Pola formularza zastępstw" }
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

            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role { Name = "Admin" };
                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }

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
                    RoleId = adminRole.Id,
                    // Domyślne hasło "admin" jest publiczne - wymuszamy zmianę przy pierwszym logowaniu.
                    MustChangePassword = true,
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }

            if (admin.RoleId == null)
            {
                admin.RoleId = adminRole.Id;
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
