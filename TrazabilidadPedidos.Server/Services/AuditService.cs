using System.Text.Json;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string action, string entity, int? entityId, object? oldValues, object? newValues, string? ipAddress);
        Task<List<AuditLog>> GetAllAsync(int? userId, string? action, string? entity, int page, int pageSize);
        Task<AuditLog?> GetByIdAsync(int id);
        Task<int> GetCountAsync(int? userId, string? action, string? entity);
    }

    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repository;

        public AuditService(IAuditRepository repository)
        {
            _repository = repository;
        }

        public async Task LogAsync(int? userId, string action, string entity, int? entityId, object? oldValues, object? newValues, string? ipAddress)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            };

            await _repository.AddAsync(log);
        }

        public async Task<List<AuditLog>> GetAllAsync(int? userId, string? action, string? entity, int page, int pageSize)
        {
            return await _repository.GetAllAsync(userId, action, entity, page, pageSize);
        }

        public async Task<AuditLog?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<int> GetCountAsync(int? userId, string? action, string? entity)
        {
            return await _repository.GetCountAsync(userId, action, entity);
        }
    }
}
