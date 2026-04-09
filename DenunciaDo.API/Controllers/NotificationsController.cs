using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
       
        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetUserNotifications()
        {
            var userId = GetUserId();
            var result = await _notificationService.GetNotificationsByUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationCount()
        {
            var userId = GetUserId();
            var result = await _notificationService.GetUnreadNotificationCountByUserAsync(userId);
            return Ok(result);
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<ActionResult> MarkNotificationAsRead(int id)
        {
            var userId = GetUserId();
            var result = await _notificationService.MarkNotificationAsReadAsync(id, userId);

            if (!result)
            {
                return BadRequest(new { message = "Mark notification as read failed" });
            }

            return Ok(new { message = "Notification marked as read" });
        }

        [HttpPut("mark-all-as-read")]
        public async Task<ActionResult> MarkAllNotificationsAsRead()
        {
            var userId = GetUserId();
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            if (!result)
            {
                return BadRequest(new { message = "Mark all notifications as read failed" });
            }

            return Ok(new { message = "All notifications marked as read" });
        }
    }
}
