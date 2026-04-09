using AutoMapper;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<NotificationDto>> GetNotificationsByUserAsync(int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetNotificationsByUserAsync(userId);
            return _mapper.Map<List<NotificationDto>>(notifications);
        }

        public async Task<int> GetUnreadNotificationCountByUserAsync(int userId)
        {
            return await _unitOfWork.Notifications.GetUnreadNotificationCountByUserAsync(userId);
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto)
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsActive = true;
            notification.IsRead = false;

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null || notification.UserId != userId) return false;

            await _unitOfWork.Notifications.MarkNotificationAsReadAsync(notificationId);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
        {
            await _unitOfWork.Notifications.MarkAllNotificationsAsReadAsync(userId);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
