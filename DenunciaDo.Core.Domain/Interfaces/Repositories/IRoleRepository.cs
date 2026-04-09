using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Domain.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetRoleByNameAsync(string name);
    }
}
