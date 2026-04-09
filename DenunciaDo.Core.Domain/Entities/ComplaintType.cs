using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class ComplaintType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        // Relaciones Uno a Muchos
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
