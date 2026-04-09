using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class Complaint : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Image { get; set; }

        // Llaves foráneas
        public int UserId { get; set; }
        public int ComplaintTypeId { get; set; }
        public int StatusId { get; set; }
        public int? DistrictId { get; set; }

        // Relaciones Uno a Muchos (extremo muchos)
        public virtual User User { get; set; }
        public virtual ComplaintType ComplaintType { get; set; }
        public virtual Status Status { get; set; }
        public virtual District District { get; set; }

        // Relaciones Uno a Muchos (extremo uno)
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        // Relaciones Muchos a Muchos
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<ComplaintHistory> History { get; set; } = new List<ComplaintHistory>();
    }
}
