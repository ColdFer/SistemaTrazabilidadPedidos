using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly AppDbContext _context;

        public DeliveryRepository(AppDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DESPACHOS
        // =====================================================

        public async Task<List<Delivery>> GetAllAsync()
        {
            return await _context.Deliveries

            .Include(d => d.Order)
                .ThenInclude(o => o!.Customer)
                    .ThenInclude(c => c!.User)

            .Include(d => d.Address)

            .Include(d => d.DeliveryDriver)
                .ThenInclude(driver => driver!.User)

            .OrderByDescending(d => d.CreatedAt)

            .ToListAsync();
        }


        public async Task<Delivery?> GetByIdAsync(int id)
        {
            return await _context.Deliveries

            .Include(d => d.Order)
                .ThenInclude(o => o!.Customer)
                    .ThenInclude(c => c!.User)

            .Include(d => d.Address)

            .Include(d => d.DeliveryDriver)
                .ThenInclude(driver => driver!.User)

            .FirstOrDefaultAsync(d => d.Id == id);
        }


        public async Task<bool> DeliveryExistsForOrderAsync(
            int orderId)
        {
            return await _context.Deliveries
                .AnyAsync(d => d.OrderId == orderId);
        }


        public async Task AddAsync(Delivery delivery)
        {
            await _context.Deliveries.AddAsync(delivery);
        }


        // =====================================================
        // PEDIDOS
        // =====================================================

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _context.Orders
            .Include(o => o.Customer)
                .ThenInclude(c => c!.User)
            .Include(o => o.CurrentStatus)
            .Where(o => o.CurrentStatus != null
                && o.CurrentStatus.Name == "ListoParaEntrega"
                && _context.Payments.Any(p =>
                    p.OrderId == o.Id
                    && p.Status == Shared.Enums.PaymentStatus.Confirmed))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        }


        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
            .Include(o => o.Customer)
                .ThenInclude(c => c!.User)
            .FirstOrDefaultAsync(o => o.Id == id);
        }


        // =====================================================
        // DIRECCIONES
        // =====================================================

        public async Task<List<Address>> GetAddressesAsync()
        {
            return await _context.Addresses
            .Include(a => a.Customer)
                .ThenInclude(c => c!.User)
            .Where(a => a.IsActive)
            .OrderBy(a => a.AddressLine)
            .ToListAsync();
        }


        public async Task<Address?> GetAddressByIdAsync(int id)
        {
            return await _context.Addresses
            .Include(a => a.Customer)
                .ThenInclude(c => c!.User)
            .FirstOrDefaultAsync(a => a.Id == id);
        }


        // =====================================================
        // REPARTIDORES
        // =====================================================

        public async Task<List<DeliveryDriver>> GetDriversAsync()
        {
            return await _context.DeliveryDrivers
            .Include(d => d.User)
            .OrderBy(d => d.User!.FirstName)
            .ThenBy(d => d.User!.LastName)
            .ToListAsync();
        }


        public async Task<DeliveryDriver?> GetDriverByIdAsync(
            int id)
        {
            return await _context.DeliveryDrivers
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
        }


        // =====================================================
        // GUARDADO
        // =====================================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}