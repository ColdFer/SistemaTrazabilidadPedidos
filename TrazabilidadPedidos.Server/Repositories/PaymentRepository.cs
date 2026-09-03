using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o!.Customer)
                        .ThenInclude(c => c!.User)
                .Include(p => p.VerifiedByUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o!.Customer)
                        .ThenInclude(c => c!.User)
                .Include(p => p.VerifiedByUser)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPendingAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o!.Customer)
                        .ThenInclude(c => c!.User)
                .Where(p => p.Status == PaymentStatus.Pending)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Where(p => p.Order!.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
