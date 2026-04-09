using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Attachment>> GetAttachmentsByComplaintAsync(int complaintId)
        {
            return await _dbContext.Attachments
                .Where(a => a.ComplaintId == complaintId && a.IsActive)
                .ToListAsync();
        }
    }
}
