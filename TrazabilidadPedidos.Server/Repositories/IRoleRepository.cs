using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetWithPermissionsAsync(int id);
        Task<bool> HasUsersAsync(int id);
        Task<bool> HasDuplicateNameAsync(string name, int? excludeId = null);
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task SaveChangesAsync();
    }
}
