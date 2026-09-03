using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetByOrderIdAsync(int orderId);
        Task<List<Invoice>> GetAllAsync();
        Task<List<Invoice>> GetByCustomerIdAsync(int customerId);
        Task AddAsync(Invoice invoice);
        Task SaveChangesAsync();
    }

    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                .Include(i => i.Customer)
                    .ThenInclude(c => c!.User)
                .Include(i => i.Details)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Invoices
                .Include(i => i.Details)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Order)
                .Include(i => i.Customer)
                    .ThenInclude(c => c!.User)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
