using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using System.Linq.Expressions;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        public PermissionRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<EnumPermissions>> GetUserPermissionsAsync(int userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.Permission.PermissionName)
                .ToListAsync();
        }
        public async Task<Dictionary<EnumPermissions, bool>> GetAllUserPermissionsAsync(int userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var permissionsList = await GetUserPermissionsAsync(userId);
            return Enum.GetValues<EnumPermissions>()
        .ToDictionary(
            permission => permission,
            permission => permissionsList.Contains(permission)
        );

        }
        public async Task<bool> HasPermissionAsync(ClaimsPrincipal principal, EnumPermissions permission)
        {

            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            if (!int.TryParse(id, out var permissionId))
            {
                return false;
            }
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.UserPermissions
             .AnyAsync(up =>
                 up.UserId == permissionId &&
                 (up.Permission.PermissionName == permission ||
                  up.Permission.PermissionName == EnumPermissions.Master));
        }
        public async Task<bool> SavePermissionsAsync(List<EnumPermissions> permissions, int userId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {

                var masterId = await _context.Permissions
                    .Where(p => p.PermissionName == EnumPermissions.Master)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
                var existingPermissions = _context.UserPermissions
                    .Where(up => up.UserId == userId && up.PermissionId != masterId);

                _context.UserPermissions.RemoveRange(existingPermissions);
                foreach (var permission in permissions)
                {
                    var permissionId = await _context.Permissions
                        .Where(p => p.PermissionName == permission)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    if (permissionId == 0)
                        continue;
                    _context.UserPermissions.Add(new UserPermission
                    {
                        UserId = userId,
                        PermissionId = permissionId,
                        PermissionName = permission.ToString()
                    });
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}