using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class Vote : BaseEntity
    {
        public int UserId { get; set; }
        public int ComplaintId { get; set; }
        public bool IsUpvote { get; set; }

        // Relaciones Muchos a Muchos (tablas de unión)
        public virtual User User { get; set; }
        public virtual Complaint Complaint { get; set; }
    }
}
