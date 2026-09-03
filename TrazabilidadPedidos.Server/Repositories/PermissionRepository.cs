using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Permission?> GetByNameAsync(string name)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> HasDuplicateNameAsync(string name, int? excludeId = null)
        {
            return await _context.Permissions
                .AnyAsync(p => p.Name == name && (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task<bool> IsAssignedToAnyRoleAsync(int id)
        {
            return await _context.RolePermissions.AnyAsync(rp => rp.PermissionId == id);
        }

        public async Task AddAsync(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Permission permission)
        {
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
