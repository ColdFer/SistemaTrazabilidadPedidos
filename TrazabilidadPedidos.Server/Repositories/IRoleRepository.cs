using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);

        Task<Role?> GetByNameAsync(string name);
    }
}