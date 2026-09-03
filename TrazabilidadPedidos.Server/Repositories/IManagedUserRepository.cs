using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IManagedUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task<bool> EmailExistsAsync(
            string email,
            int? excludeUserId = null);

        Task<DeliveryDriver?> GetDriverByUserIdAsync(
            int userId);

        Task AddUserAsync(User user);

        Task AddDriverAsync(DeliveryDriver driver);

        Task SaveChangesAsync();
    }
}