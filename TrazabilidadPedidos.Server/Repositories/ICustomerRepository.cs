using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<bool> CiExistsAsync(
            string ci,
            int? excludeCustomerId = null);

        Task<bool> EmailExistsAsync(
            string email,
            int? excludeUserId = null);

        Task AddAsync(Customer customer);

        Task SaveChangesAsync();
    }
}