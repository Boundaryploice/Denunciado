using DenunciaDo.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetNotificationsByUserAsync(int userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadNotificationCountByUserAsync(int userId)
        {
            return await _dbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead && n.IsActive);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _dbContext.Entry(notification).State = EntityState.Modified;
            }
        }

        public async Task MarkAllNotificationsAsReadAsync(int userId)
        {
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && n.IsActive)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _dbContext.Entry(notification).State = EntityState.Modified;
            }
        }
    }
}
