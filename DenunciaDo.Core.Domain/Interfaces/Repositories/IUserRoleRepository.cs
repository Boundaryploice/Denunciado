using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Repositories
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<IReadOnlyList<UserRole>> GetUserRolesByUserIdAsync(int userId);
        Task<IReadOnlyList<UserRole>> GetUserRolesByRoleIdAsync(int roleId);
    }
}
