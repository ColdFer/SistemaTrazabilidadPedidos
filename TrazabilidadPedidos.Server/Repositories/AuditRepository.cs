using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IAuditRepository
    {
        Task<List<AuditLog>> GetAllAsync(int? userId, string? action, string? entity, int page, int pageSize);
        Task<AuditLog?> GetByIdAsync(int id);
        Task<int> GetCountAsync(int? userId, string? action, string? entity);
        Task AddAsync(AuditLog log);
        Task SaveChangesAsync();
    }

    public class AuditRepository : IAuditRepository
    {
        private readonly AppDbContext _context;

        public AuditRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAllAsync(int? userId, string? action, string? entity, int page, int pageSize)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            if (!string.IsNullOrEmpty(entity))
                query = query.Where(a => a.Entity == entity);

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<AuditLog?> GetByIdAsync(int id)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> GetCountAsync(int? userId, string? action, string? entity)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            if (!string.IsNullOrEmpty(entity))
                query = query.Where(a => a.Entity == entity);

            return await query.CountAsync();
        }

        public async Task AddAsync(AuditLog log)
        {
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
