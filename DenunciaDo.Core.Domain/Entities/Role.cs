namespace DenunciaDo.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Description { get; set; }

        // Relaciones Muchos a Muchos
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
