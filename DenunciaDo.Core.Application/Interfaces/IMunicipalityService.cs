using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Application.Interfaces
{
    public interface IMunicipalityService
    {
        Task<List<MunicipalityDto>> GetAllMunicipalitiesAsync();
        Task<MunicipalityDto> GetMunicipalityByIdAsync(int id);
        Task<MunicipalityDto> GetMunicipalityWithDistrictsAsync(int id);
        Task<MunicipalityDto> CreateMunicipalityAsync(MunicipalityDto municipalityDto);
        Task<MunicipalityDto> UpdateMunicipalityAsync(MunicipalityDto municipalityDto);
        Task<bool> DeleteMunicipalityAsync(int id);
    }
}
