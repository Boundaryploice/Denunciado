using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Application.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsByUserAsync(int userId);
        Task<int> GetUnreadNotificationCountByUserAsync(int userId);
        Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto);
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllNotificationsAsReadAsync(int userId);
    }
}
