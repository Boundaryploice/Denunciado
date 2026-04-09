using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Core.Domain.Interfaces.Repositories
{
    public interface IVoteRepository : IRepository<Vote>
    {
        Task<Vote> GetVoteByUserAndComplaintAsync(int userId, int complaintId);
        Task<IReadOnlyList<Vote>> GetVotesByUserAsync(int userId);
        Task<IReadOnlyList<Vote>> GetVotesByComplaintAsync(int complaintId);
        Task<int> GetUpvoteCountByComplaintAsync(int complaintId);
        Task<int> GetDownvoteCountByComplaintAsync(int complaintId);
    }
}
