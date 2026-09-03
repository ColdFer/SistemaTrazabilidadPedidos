using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.User)
                .OrderBy(c => c.User!.FirstName)
                .ThenBy(c => c.User!.LastName)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> CiExistsAsync(
            string ci,
            int? excludeCustomerId = null)
        {
            return await _context.Customers
                .AnyAsync(c =>
                    c.Ci == ci &&
                    (!excludeCustomerId.HasValue ||
                     c.Id != excludeCustomerId.Value));
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeUserId = null)
        {
            return await _context.Users
                .AnyAsync(u =>
                    u.Email == email &&
                    (!excludeUserId.HasValue ||
                     u.Id != excludeUserId.Value));
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}