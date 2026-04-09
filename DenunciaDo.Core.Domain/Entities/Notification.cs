using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
        public string RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }

        // Relaciones
        public virtual User User { get; set; }
    }
}
