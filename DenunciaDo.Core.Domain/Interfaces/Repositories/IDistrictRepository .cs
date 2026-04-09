using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Interfaces.Repositories
{
    public interface IDistrictRepository : IRepository<District>
    {
        Task<IReadOnlyList<District>> GetDistrictsByMunicipalityAsync(int municipalityId);
    }
}
