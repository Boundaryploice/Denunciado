using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface IComplaintService
    {
        Task<ComplaintDetailDto> GetComplaintByIdAsync(int id, int? userId = null);
        Task<PaginatedListDto<ComplaintDto>> GetPaginatedComplaintsAsync(int pageIndex, int pageSize, int? userId = null);
        Task<PaginatedListDto<ComplaintDto>> GetComplaintsByTypeAsync(int typeId, int pageIndex, int pageSize, int? userId = null);
        Task<PaginatedListDto<ComplaintDto>> GetComplaintsByStatusAsync(int statusId, int pageIndex, int pageSize, int? userId = null);
        Task<PaginatedListDto<ComplaintDto>> GetComplaintsByDistrictAsync(int districtId, int pageIndex, int pageSize, int? userId = null);
        Task<PaginatedListDto<ComplaintDto>> GetComplaintsByUserAsync(int userId, int pageIndex, int pageSize);
        Task<List<ComplaintDto>> GetRecentComplaintsAsync(int count, int? userId = null);
        Task<List<ComplaintDto>> GetTopVotedComplaintsAsync(int count, int? userId = null);
        Task<ComplaintDto> CreateComplaintAsync(int userId, CreateComplaintDto createDto);
        Task<ComplaintDto> UpdateComplaintAsync(int userId, UpdateComplaintDto updateDto);
        Task<bool> DeleteComplaintAsync(int id, int userId);
        Task<ComplaintDto> UpdateComplaintStatusAsync(int userId, UpdateComplaintStatusDto updateDto);
        Task<PaginatedListDto<ComplaintDto>> SearchComplaintsAsync(string searchTerm, int pageIndex, int pageSize, int? userId = null);
    }
}
