using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public int UserId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Identification { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Bio { get; set; }

        // Relación Uno a Uno
        public virtual User User { get; set; }
    }
}
