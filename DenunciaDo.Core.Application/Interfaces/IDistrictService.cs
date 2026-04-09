using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Application.Interfaces
{
    public interface IDistrictService
    {
        Task<List<DistrictDto>> GetAllDistrictsAsync();
        Task<DistrictDto> GetDistrictByIdAsync(int id);
        Task<List<DistrictDto>> GetDistrictsByMunicipalityAsync(int municipalityId);
        Task<DistrictDto> CreateDistrictAsync(DistrictDto districtDto);
        Task<DistrictDto> UpdateDistrictAsync(DistrictDto districtDto);
        Task<bool> DeleteDistrictAsync(int id);
    }
}
