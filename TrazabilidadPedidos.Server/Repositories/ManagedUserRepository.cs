using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class ManagedUserRepository
        : IManagedUserRepository
    {
        private readonly AppDbContext _context;

        public ManagedUserRepository(
            AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(
                    u => u.Id == id);
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


        public async Task<DeliveryDriver?>
            GetDriverByUserIdAsync(int userId)
        {
            return await _context.DeliveryDrivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(
                    d => d.UserId == userId);
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }


        public async Task AddDriverAsync(
            DeliveryDriver driver)
        {
            await _context.DeliveryDrivers
                .AddAsync(driver);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}