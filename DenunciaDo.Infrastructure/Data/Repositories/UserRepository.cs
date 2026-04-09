using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Where(u => u.Email.ToLower() == email.ToLower() && u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByNicknameAsync(string nickname)
        {
            return await _dbContext.Users
                .Where(u => u.NickName.ToLower() == nickname.ToLower() && u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserWithProfileAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.Profile)
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserWithRolesAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName) && u.IsActive)
                .ToListAsync();
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> CheckNicknameExistsAsync(string nickname)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.NickName.ToLower() == nickname.ToLower());
        }
    }
}
