using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;

namespace DenunciaDo.Domain.Interfaces.Repositories
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        Task<IReadOnlyList<Attachment>> GetAttachmentsByComplaintAsync(int complaintId);
    }
}
