using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public int ComplaintId { get; set; }

        // Relaciones Uno a Muchos (extremo muchos)
        public virtual Complaint Complaint { get; set; }
    }
}
