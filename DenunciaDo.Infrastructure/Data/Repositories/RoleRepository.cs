using DenunciaDo.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await _dbContext.Roles
                .Where(r => r.Name.ToLower() == name.ToLower() && r.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}
