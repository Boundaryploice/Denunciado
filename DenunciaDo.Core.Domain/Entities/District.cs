using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Entities
{
    public class District : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int MunicipalityId { get; set; }

        // Relaciones Uno a Muchos (extremo muchos)
        public virtual Municipality Municipality { get; set; }

        // Relaciones Uno a Muchos (extremo uno)
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
