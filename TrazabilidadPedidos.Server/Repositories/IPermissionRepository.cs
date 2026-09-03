using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(int id);
        Task<Permission?> GetByNameAsync(string name);
        Task<List<Permission>> GetAllAsync();
        Task<bool> HasDuplicateNameAsync(string name, int? excludeId = null);
        Task<bool> IsAssignedToAnyRoleAsync(int id);
        Task AddAsync(Permission permission);
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(Permission permission);
        Task SaveChangesAsync();
    }
}
