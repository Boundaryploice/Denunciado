using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Core.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByNicknameAsync(string nickname);
        Task<User> GetUserWithProfileAsync(int userId);
        Task<User> GetUserWithRolesAsync(int userId);
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckNicknameExistsAsync(string nickname);
    }
}
