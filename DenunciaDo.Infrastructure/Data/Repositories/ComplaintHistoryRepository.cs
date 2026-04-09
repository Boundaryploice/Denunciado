using DenunciaDo.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class ComplaintHistoryRepository : Repository<ComplaintHistory>, IComplaintHistoryRepository
    {
        public ComplaintHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<ComplaintHistory>> GetHistoryByComplaintAsync(int complaintId)
        {
            return await _dbContext.ComplaintHistories
                .Include(ch => ch.User)
                .Include(ch => ch.Status)
                .Where(ch => ch.ComplaintId == complaintId && ch.IsActive)
                .OrderBy(ch => ch.CreatedAt)
                .ToListAsync();
        }
    }
}
