using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Core.Domain.Interfaces.Repositories
{
    public interface IComplaintRepository : IRepository<Complaint>
    {
        Task<Complaint> GetComplaintWithDetailsAsync(int id);
        Task<IReadOnlyList<Complaint>> GetComplaintsByUserAsync(int userId);
        Task<IReadOnlyList<Complaint>> GetComplaintsByStatusAsync(int statusId);
        Task<IReadOnlyList<Complaint>> GetComplaintsByTypeAsync(int typeId);
        Task<IReadOnlyList<Complaint>> GetComplaintsByDistrictAsync(int districtId);
        Task<IReadOnlyList<Complaint>> GetRecentComplaintsAsync(int count);
        Task<IReadOnlyList<Complaint>> GetTopVotedComplaintsAsync(int count);
    }
}
