using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class MunicipalityRepository : Repository<Municipality>, IMunicipalityRepository
    {
        public MunicipalityRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Municipality> GetMunicipalityWithDistrictsAsync(int municipalityId)
        {
            return await _dbContext.Municipalities
                .Include(m => m.Districts)
                .Where(m => m.Id == municipalityId && m.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}
