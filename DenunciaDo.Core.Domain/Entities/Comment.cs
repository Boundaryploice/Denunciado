using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public int UserId { get; set; }
        public int ComplaintId { get; set; }
        public int? ParentCommentId { get; set; }

        // Relaciones Muchos a Muchos (tablas de unión)
        public virtual User User { get; set; }
        public virtual Complaint Complaint { get; set; }

        // Auto-relación para respuestas a comentarios
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
