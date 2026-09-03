using TrazabilidadPedidos.Shared.DTOs.Orders;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<List<OrderDto>> GetByCustomerIdAsync(int customerId);
        Task<OrderDto?> GetByIdAsync(int id);
        Task<OrderDto?> CreateAsync(CreateOrderRequest request);
        Task<bool> AcceptOrderAsync(int orderId, int userId);
        Task<bool> UpdateStatusAsync(int orderId, int statusId, string? observation, int userId);
        Task<List<OrderStatusDto>> GetStatusesAsync();
    }
}
