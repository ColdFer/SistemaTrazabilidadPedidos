using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.User)
                .Include(o => o.CurrentStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.CurrentStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.User)
                .Include(o => o.CurrentStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<OrderStatus>> GetStatusesAsync()
        {
            return await _context.OrderStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        public async Task<OrderStatus?> GetStatusByNameAsync(string name)
        {
            return await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task AddStatusHistoryAsync(OrderStatusHistory history)
        {
            await _context.OrderStatusHistories.AddAsync(history);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
