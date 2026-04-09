using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class ComplaintRepository : Repository<Complaint>, IComplaintRepository
    {
        public ComplaintRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Complaint> GetComplaintWithDetailsAsync(int id)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Include(c => c.Attachments)
                .Include(c => c.Votes)
                .Include(c => c.Comments)
                    .ThenInclude(cm => cm.User)
                .Include(c => c.History)
                    .ThenInclude(h => h.Status)
                .Include(c => c.History)
                    .ThenInclude(h => h.User)
                .Where(c => c.Id == id && c.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Complaint>> GetComplaintsByUserAsync(int userId)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Complaint>> GetComplaintsByStatusAsync(int statusId)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Where(c => c.StatusId == statusId && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Complaint>> GetComplaintsByTypeAsync(int typeId)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Where(c => c.ComplaintTypeId == typeId && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Complaint>> GetComplaintsByDistrictAsync(int districtId)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Where(c => c.DistrictId == districtId && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Complaint>> GetRecentComplaintsAsync(int count)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Complaint>> GetTopVotedComplaintsAsync(int count)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                .Include(c => c.Votes)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Votes.Count(v => v.IsUpvote))
                .Take(count)
                .ToListAsync();
        }
    }
}
