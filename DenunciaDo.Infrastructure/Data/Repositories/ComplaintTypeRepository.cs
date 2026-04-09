using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class ComplaintTypeRepository : Repository<ComplaintType>, IComplaintTypeRepository
    {
        public ComplaintTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
