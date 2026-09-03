using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByCustomerIdAsync(int customerId);
        Task<Order?> GetByIdAsync(int id);
        Task<List<OrderStatus>> GetStatusesAsync();
        Task<OrderStatus?> GetStatusByNameAsync(string name);
        Task AddAsync(Order order);
        Task AddStatusHistoryAsync(OrderStatusHistory history);
        Task SaveChangesAsync();
    }
}
