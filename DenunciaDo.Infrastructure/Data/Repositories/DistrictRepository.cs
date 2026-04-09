using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class DistrictRepository : Repository<District>, IDistrictRepository
    {
        public DistrictRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<District>> GetDistrictsByMunicipalityAsync(int municipalityId)
        {
            return await _dbContext.Districts
                .Where(d => d.MunicipalityId == municipalityId && d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}
