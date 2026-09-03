using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetPendingAsync();
        Task<List<Payment>> GetByCustomerIdAsync(int customerId);
        Task AddAsync(Payment payment);
        Task SaveChangesAsync();
    }
}
