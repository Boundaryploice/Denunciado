using DenunciaDo.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<UserRole>> GetUserRolesByUserIdAsync(int userId)
        {
            return await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UserRole>> GetUserRolesByRoleIdAsync(int roleId)
        {
            return await _dbContext.UserRoles
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .ToListAsync();
        }
    }
}
