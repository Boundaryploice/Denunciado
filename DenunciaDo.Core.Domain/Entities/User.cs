using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string PasswordHash { get; set; }
        public string Picture { get; set; }
        public string DeviceId { get; set; }
        public bool IsAnonymous { get; set; }

        // Relación Uno a Uno
        public UserProfile Profile { get; set; }

        // Relaciones Uno a Muchos
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        // Relaciones Muchos a Muchos
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public string FullName => $"{FirstName} {LastName}";
    }

}
