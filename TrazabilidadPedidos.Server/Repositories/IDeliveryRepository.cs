using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IDeliveryRepository
    {
        // DESPACHOS
        Task<List<Delivery>> GetAllAsync();

        Task<Delivery?> GetByIdAsync(int id);

        Task<bool> DeliveryExistsForOrderAsync(int orderId);

        Task AddAsync(Delivery delivery);


        // PEDIDOS
        Task<List<Order>> GetOrdersAsync();

        Task<Order?> GetOrderByIdAsync(int id);


        // DIRECCIONES
        Task<List<Address>> GetAddressesAsync();

        Task<Address?> GetAddressByIdAsync(int id);


        // REPARTIDORES
        Task<List<DeliveryDriver>> GetDriversAsync();

        Task<DeliveryDriver?> GetDriverByIdAsync(int id);


        // GUARDADO
        Task SaveChangesAsync();
    }
}