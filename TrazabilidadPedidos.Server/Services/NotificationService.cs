using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task CreateAsync(int userId, string title, string message, string? type, int? relatedId);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _repository.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _repository.GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _repository.SaveChangesAsync();
            }
        }

        public async Task CreateAsync(
            int userId, string title, string message, string? type, int? relatedId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedId = relatedId,
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            await _repository.AddAsync(notification);
        }
    }
}
