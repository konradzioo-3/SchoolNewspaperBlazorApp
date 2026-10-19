using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using System.Security;
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
        public async Task<bool> HasPermissionAsync(ClaimsPrincipal principal, EnumPermission permissions)
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
            return await _context.UserPermisions
                .AnyAsync(up => up.UserId == permissionId &&
                up.Permission.PermissionName == permissions);
        }

        public async Task SavePermitionsAsync(List<EnumPermission> permissions, int userId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var existingPermissions = _context.UserPermisions
                .Where(p => p.UserId == userId);
            _context.UserPermisions.RemoveRange(existingPermissions);
            foreach (var permission in permissions)
            {
                var permissionId = await _context.Permissions
                    .Where(p => p.PermissionName == permission)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
                if (permissionId == 0)
                    continue;
                _context.UserPermisions.Add(new UserPermision
                {
                    UserId = userId,
                    PermissionId = permissionId
                });
                await _context.SaveChangesAsync();

            }
        }
    }

}
