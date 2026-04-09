using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Entities
{
    public class ComplaintHistory : BaseEntity
    {
        public int ComplaintId { get; set; }
        public int? UserId { get; set; }
        public int StatusId { get; set; }
        public string Comments { get; set; }

        // Relaciones
        public virtual Complaint Complaint { get; set; }
        public virtual User User { get; set; }
        public virtual Status Status { get; set; }
    }
}
