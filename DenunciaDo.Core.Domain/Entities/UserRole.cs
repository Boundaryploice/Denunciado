using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Relaciones Muchos a Muchos (tablas de unión)
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
