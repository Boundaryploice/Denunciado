using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class StatusRepository : Repository<Status>, IStatusRepository
    {
        public StatusRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
