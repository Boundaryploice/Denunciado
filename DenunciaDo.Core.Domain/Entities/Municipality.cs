namespace DenunciaDo.Domain.Entities
{
    public class Municipality : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }

        // Relaciones Uno a Muchos
        public virtual ICollection<District> Districts { get; set; } = new List<District>();
    }
}
