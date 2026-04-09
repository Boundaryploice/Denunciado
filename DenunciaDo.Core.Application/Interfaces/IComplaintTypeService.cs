using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface IComplaintTypeService
    {
        Task<List<ComplaintTypeDto>> GetAllComplaintTypesAsync();
        Task<ComplaintTypeDto> GetComplaintTypeByIdAsync(int id);
        Task<ComplaintTypeDto> CreateComplaintTypeAsync(ComplaintTypeDto complaintTypeDto);
        Task<ComplaintTypeDto> UpdateComplaintTypeAsync(ComplaintTypeDto complaintTypeDto);
        Task<bool> DeleteComplaintTypeAsync(int id);
    }
}
