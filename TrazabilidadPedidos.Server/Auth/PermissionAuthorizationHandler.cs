using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;

namespace TrazabilidadPedidos.Server.Auth
{
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AppDbContext _context;

        public PermissionAuthorizationHandler(AppDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            try
            {
                var userId = context.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) ||
                    !int.TryParse(userId, out var userIdInt))
                {
                    return;
                }

                var user = await _context.Users
                    .Include(u => u.Role)
                        .ThenInclude(r => r!.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Id == userIdInt);

                if (user?.Role == null)
                    return;

                if (!user.Role.IsActive)
                    return;

                if (user.Role.RolePermissions.Any(rp =>
                    rp.Permission != null &&
                    rp.Permission.Name == requirement.Permission))
                {
                    context.Succeed(requirement);
                }
            }
            catch
            {
            }
        }
    }
}
