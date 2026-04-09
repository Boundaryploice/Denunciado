using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
