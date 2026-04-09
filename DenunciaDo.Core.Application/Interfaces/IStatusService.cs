using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface IStatusService
    {
        Task<List<StatusDto>> GetAllStatusesAsync();
        Task<StatusDto> GetStatusByIdAsync(int id);
        Task<StatusDto> CreateStatusAsync(StatusDto statusDto);
        Task<StatusDto> UpdateStatusAsync(StatusDto statusDto);
        Task<bool> DeleteStatusAsync(int id);
    }
}
