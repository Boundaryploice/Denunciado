using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Core.Domain.Interfaces.Repositories
{
    public interface IMunicipalityRepository : IRepository<Municipality>
    {
        Task<Municipality> GetMunicipalityWithDistrictsAsync(int municipalityId);
    }
}
