namespace DenunciaDo.Core.Application.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
        public string RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
