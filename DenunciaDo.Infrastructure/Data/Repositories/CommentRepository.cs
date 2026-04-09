using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Comment>> GetCommentsByUserAsync(int userId)
        {
            return await _dbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Complaint)
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Comment>> GetCommentsByComplaintAsync(int complaintId)
        {
            return await _dbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Where(c => c.ComplaintId == complaintId && c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Comment>> GetRepliesByCommentAsync(int commentId)
        {
            return await _dbContext.Comments
                .Include(c => c.User)
                .Where(c => c.ParentCommentId == commentId && c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
