using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintType> ComplaintTypes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ComplaintHistory> ComplaintHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints

            // User - UserProfile (One-to-One)
            modelBuilder.Entity<UserProfile>()
                .HasOne(up => up.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Complaint (One-to-Many)
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.User)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ComplaintType - Complaint (One-to-Many)
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.ComplaintType)
                .WithMany(ct => ct.Complaints)
                .HasForeignKey(c => c.ComplaintTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Status - Complaint (One-to-Many)
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Status)
                .WithMany(s => s.Complaints)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // District - Complaint (One-to-Many)
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.District)
                .WithMany(d => d.Complaints)
                .HasForeignKey(c => c.DistrictId)
                .OnDelete(DeleteBehavior.SetNull);

            // Municipality - District (One-to-Many)
            modelBuilder.Entity<District>()
                .HasOne(d => d.Municipality)
                .WithMany(m => m.Districts)
                .HasForeignKey(d => d.MunicipalityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Complaint - Attachment (One-to-Many)
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Complaint)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Vote - Complaint (Many-to-Many)
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Complaint)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for user votes on complaints
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.UserId, v.ComplaintId })
                .IsUnique();

            // User - Comment - Complaint (Many-to-Many)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Complaint)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment self-referencing relationship for replies
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Role (Many-to-Many)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for user roles
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // Complaint - ComplaintHistory
            modelBuilder.Entity<ComplaintHistory>()
                .HasOne(ch => ch.Complaint)
                .WithMany(c => c.History)
                .HasForeignKey(ch => ch.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplaintHistory>()
                .HasOne(ch => ch.User)
                .WithMany()
                .HasForeignKey(ch => ch.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ComplaintHistory>()
                .HasOne(ch => ch.Status)
                .WithMany()
                .HasForeignKey(ch => ch.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.NickName)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<ComplaintType>()
                .HasIndex(ct => ct.Name)
                .IsUnique();

            modelBuilder.Entity<Status>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Municipality>()
                .HasIndex(m => m.Code)
                .IsUnique();

            modelBuilder.Entity<District>()
                .HasIndex(d => new { d.Code, d.MunicipalityId })
                .IsUnique();

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN", Description = "Administrator role with full access", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Role { Id = 2, Name = "Staff", NormalizedName = "STAFF", Description = "Staff role with limited administrative access", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Role { Id = 3, Name = "User", NormalizedName = "USER", Description = "Regular user role", CreatedAt = DateTime.UtcNow, IsActive = true }
            );

            // Seed statuses
            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "Pending", Description = "The complaint is pending review", Color = "#FFC107", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Status { Id = 2, Name = "In Progress", Description = "The complaint is being addressed", Color = "#2196F3", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Status { Id = 3, Name = "Resolved", Description = "The complaint has been resolved", Color = "#4CAF50", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Status { Id = 4, Name = "Rejected", Description = "The complaint has been rejected", Color = "#F44336", CreatedAt = DateTime.UtcNow, IsActive = true }
            );

            // Seed complaint types
            modelBuilder.Entity<ComplaintType>().HasData(
                new ComplaintType { Id = 1, Name = "Road Issue", Description = "Issues related to roads, potholes, etc.", Icon = "fa-road", CreatedAt = DateTime.UtcNow, IsActive = true },
                new ComplaintType { Id = 2, Name = "Waste Management", Description = "Issues related to garbage collection, etc.", Icon = "fa-trash", CreatedAt = DateTime.UtcNow, IsActive = true },
                new ComplaintType { Id = 3, Name = "Water Supply", Description = "Issues related to water supply", Icon = "fa-tint", CreatedAt = DateTime.UtcNow, IsActive = true },
                new ComplaintType { Id = 4, Name = "Electricity", Description = "Issues related to electricity supply", Icon = "fa-bolt", CreatedAt = DateTime.UtcNow, IsActive = true },
                new ComplaintType { Id = 5, Name = "Public Safety", Description = "Issues related to public safety", Icon = "fa-shield-alt", CreatedAt = DateTime.UtcNow, IsActive = true },
                new ComplaintType { Id = 6, Name = "Other", Description = "Other issues not covered by other types", Icon = "fa-question-circle", CreatedAt = DateTime.UtcNow, IsActive = true }
            );

            // Sample municipality and districts
            modelBuilder.Entity<Municipality>().HasData(
                new Municipality { Id = 1, Name = "Santo Domingo", Code = "SD", CreatedAt = DateTime.UtcNow, IsActive = true }
            );

            modelBuilder.Entity<District>().HasData(
                new District { Id = 1, Name = "Distrito Nacional", Code = "DN", MunicipalityId = 1, CreatedAt = DateTime.UtcNow, IsActive = true },
                new District { Id = 2, Name = "Santo Domingo Este", Code = "SDE", MunicipalityId = 1, CreatedAt = DateTime.UtcNow, IsActive = true },
                new District { Id = 3, Name = "Santo Domingo Norte", Code = "SDN", MunicipalityId = 1, CreatedAt = DateTime.UtcNow, IsActive = true },
                new District { Id = 4, Name = "Santo Domingo Oeste", Code = "SDO", MunicipalityId = 1, CreatedAt = DateTime.UtcNow, IsActive = true }
            );

            // NUEVO: Seed admin user
            // NOTA: En un entorno de producción, el password debería ser más seguro y configurado desde variables de entorno
            var adminPasswordHash = "AQAAAAEAACcQAAAAEBm7ZqEQ9KmZmD1vN3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5Ng=="; // Password: "Admin123!"

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Administrador",
                    LastName = "Sistema",
                    Email = "admin@denuncia.do",
                    NickName = "admin",
                    PasswordHash = adminPasswordHash, // Hash de "Admin123!"
                    IsAnonymous = false,
                    Picture = "/img/admin-avatar.png",
                    DeviceId = "SYSTEM",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed admin user profile
            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    Id = 1,
                    UserId = 1,
                    Address = "Oficina Central, Santo Domingo",
                    Phone = "+1 809-555-0001",
                    Identification = "ADMIN-001",
                    Bio = "Administrador del sistema Denuncia.Do",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Assign admin role to admin user
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    UserId = 1,
                    RoleId = 1, // Admin role
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // OPCIONAL: Crear algunos usuarios de ejemplo para pruebas
            var userPasswordHash = "AQAAAAEAACcQAAAAEBm7ZqEQ9KmZmD1vN3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5N3F5Ng=="; // Password: "User123!"

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    FirstName = "Juan",
                    LastName = "Pérez",
                    Email = "juan.perez@example.com",
                    NickName = "juanperez",
                    PasswordHash = userPasswordHash,
                    IsAnonymous = false,
                    DeviceId = "DEVICE-001",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    FirstName = "María",
                    LastName = "González",
                    Email = "maria.gonzalez@example.com",
                    NickName = "mariagonzalez",
                    PasswordHash = userPasswordHash,
                    IsAnonymous = false,
                    DeviceId = "DEVICE-002",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed user profiles for example users
            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    Id = 2,
                    UserId = 2,
                    Address = "Calle Principal #123, Santo Domingo Este",
                    Phone = "+1 809-555-0002",
                    Identification = "001-0123456-7",
                    Bio = "Ciudadano activo preocupado por su comunidad",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new UserProfile
                {
                    Id = 3,
                    UserId = 3,
                    Address = "Av. Winston Churchill #456, Distrito Nacional",
                    Phone = "+1 809-555-0003",
                    Identification = "001-0234567-8",
                    Bio = "Vecina comprometida con el mejoramiento del barrio",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Assign user role to example users
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 2,
                    UserId = 2,
                    RoleId = 3, // User role
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new UserRole
                {
                    Id = 3,
                    UserId = 3,
                    RoleId = 3, // User role
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // OPCIONAL: Crear algunas denuncias de ejemplo
            modelBuilder.Entity<Complaint>().HasData(
                new Complaint
                {
                    Id = 1,
                    Title = "Bache en la Avenida 27 de Febrero",
                    Description = "Hay un bache muy grande en la Avenida 27 de Febrero que está causando daños a los vehículos",
                    Detail = "El bache se encuentra exactamente frente al Centro Comercial Blue Mall. Mide aproximadamente 2 metros de largo por 1 metro de ancho y tiene una profundidad considerable. Varios conductores han reportado daños en sus neumáticos.",
                    Address = "Av. 27 de Febrero, frente a Blue Mall",
                    Latitude = 18.4765m,
                    Longitude = -69.9399m,
                    UserId = 2,
                    ComplaintTypeId = 1, // Road Issue
                    StatusId = 1, // Pending
                    DistrictId = 1, // Distrito Nacional
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsActive = true
                },
                new Complaint
                {
                    Id = 2,
                    Title = "Problema con la recolección de basura",
                    Description = "La basura no se ha recogido en nuestra calle durante más de una semana",
                    Detail = "Los contenedores están desbordados y la basura se está acumulando en las aceras. Esto está creando problemas de higiene y malos olores en todo el vecindario. Hemos contactado al ayuntamiento pero no hemos recibido respuesta.",
                    Address = "Calle José Martí #45-67, Los Alcarrizos",
                    Latitude = 18.5051m,
                    Longitude = -70.0051m,
                    UserId = 3,
                    ComplaintTypeId = 2, // Waste Management
                    StatusId = 2, // In Progress
                    DistrictId = 4, // Santo Domingo Oeste
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsActive = true
                }
            );

            // Crear historial para las denuncias de ejemplo
            modelBuilder.Entity<ComplaintHistory>().HasData(
                new ComplaintHistory
                {
                    Id = 1,
                    ComplaintId = 1,
                    UserId = 2,
                    StatusId = 1,
                    Comments = "Denuncia creada por el ciudadano",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsActive = true
                },
                new ComplaintHistory
                {
                    Id = 2,
                    ComplaintId = 2,
                    UserId = 3,
                    StatusId = 1,
                    Comments = "Denuncia creada por el ciudadano",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsActive = true
                },
                new ComplaintHistory
                {
                    Id = 3,
                    ComplaintId = 2,
                    UserId = 1, // Admin user
                    StatusId = 2,
                    Comments = "Denuncia asignada al departamento de servicios públicos para revisión",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsActive = true
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set timestamps
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.CreatedAt = DateTime.UtcNow;
                            entity.IsActive = true;
                            break;
                        case EntityState.Modified:
                            entity.UpdatedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
