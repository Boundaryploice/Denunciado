using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Services
{
    public interface IComplaintDomainService
    {
        bool CanUserEditComplaint(User user, Complaint complaint);
        bool CanUserDeleteComplaint(User user, Complaint complaint);
        bool CanUserVoteOnComplaint(User user, Complaint complaint);
        Task<bool> IsComplaintValidAsync(Complaint complaint);
    }
}
