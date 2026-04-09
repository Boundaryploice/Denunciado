using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetNotificationsByUserAsync(int userId);
        Task<int> GetUnreadNotificationCountByUserAsync(int userId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(int userId);
    }
}
