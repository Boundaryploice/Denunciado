using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Services
{
    public class ComplaintDomainService : IComplaintDomainService
    {
        public bool CanUserEditComplaint(User user, Complaint complaint)
        {
            // Solo el propietario puede editar si está en estado "Pendiente"
            if (user.Id == complaint.UserId && complaint.StatusId == 1)
                return true;

            // Los administradores y staff pueden editar
            if (user.UserRoles.Any(ur => ur.Role.Name == "Admin" || ur.Role.Name == "Staff"))
                return true;

            return false;
        }

        public bool CanUserDeleteComplaint(User user, Complaint complaint)
        {
            // Solo el propietario puede eliminar si está en estado "Pendiente"
            if (user.Id == complaint.UserId && complaint.StatusId == 1)
                return true;

            // Los administradores pueden eliminar
            if (user.UserRoles.Any(ur => ur.Role.Name == "Admin"))
                return true;

            return false;
        }

        public bool CanUserVoteOnComplaint(User user, Complaint complaint)
        {
            // No puede votar por su propia denuncia
            return user.Id != complaint.UserId;
        }

        public async Task<bool> IsComplaintValidAsync(Complaint complaint)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(complaint.Title) || complaint.Title.Length < 10)
                return false;

            if (string.IsNullOrWhiteSpace(complaint.Description) || complaint.Description.Length < 20)
                return false;

            return true;
        }
    }
}
