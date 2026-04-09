using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Repositories
{
    public interface IComplaintHistoryRepository : IRepository<ComplaintHistory>
    {
        Task<IReadOnlyList<ComplaintHistory>> GetHistoryByComplaintAsync(int complaintId);
    }
}
