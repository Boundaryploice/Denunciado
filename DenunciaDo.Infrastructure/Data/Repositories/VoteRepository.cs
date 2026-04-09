using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class VoteRepository : Repository<Vote>, IVoteRepository
    {
        public VoteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Vote> GetVoteByUserAndComplaintAsync(int userId, int complaintId)
        {
            return await _dbContext.Votes
                .Where(v => v.UserId == userId && v.ComplaintId == complaintId && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Vote>> GetVotesByUserAsync(int userId)
        {
            return await _dbContext.Votes
                .Include(v => v.Complaint)
                .Where(v => v.UserId == userId && v.IsActive)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Vote>> GetVotesByComplaintAsync(int complaintId)
        {
            return await _dbContext.Votes
                .Include(v => v.User)
                .Where(v => v.ComplaintId == complaintId && v.IsActive)
                .ToListAsync();
        }

        public async Task<int> GetUpvoteCountByComplaintAsync(int complaintId)
        {
            return await _dbContext.Votes
                .CountAsync(v => v.ComplaintId == complaintId && v.IsUpvote && v.IsActive);
        }

        public async Task<int> GetDownvoteCountByComplaintAsync(int complaintId)
        {
            return await _dbContext.Votes
                .CountAsync(v => v.ComplaintId == complaintId && !v.IsUpvote && v.IsActive);
        }
    }
}
