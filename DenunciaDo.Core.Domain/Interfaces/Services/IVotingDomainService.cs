using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Services
{
    public interface IVotingDomainService
    {
        bool CanUserVote(User user, Complaint complaint);
        Vote CreateVote(User user, Complaint complaint, bool isUpvote);
        void UpdateVote(Vote existingVote, bool isUpvote);
    }
}
