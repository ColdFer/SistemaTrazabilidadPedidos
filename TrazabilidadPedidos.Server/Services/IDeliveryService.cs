using TrazabilidadPedidos.Shared.DTOs.Dispatches;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IDeliveryService
    {
        Task<List<DeliveryDto>> GetAllAsync();

        Task<DeliveryDto?> GetByIdAsync(int id);

        Task<List<DeliveryOrderDto>> GetOrdersAsync();

        Task<List<DeliveryAddressDto>> GetAddressesAsync();

        Task<List<DeliveryDriverDto>> GetDriversAsync();

        Task<DeliveryDto?> CreateAsync(
            CreateDeliveryRequest request);

        Task<DeliveryDto?> UpdateAsync(
            int id,
            UpdateDeliveryRequest request);

        Task<DeliveryDto?> ChangeStatusAsync(
            int id,
            ChangeDeliveryStatusRequest request);
    }
}