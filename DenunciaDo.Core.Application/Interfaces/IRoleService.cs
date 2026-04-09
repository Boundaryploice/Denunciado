using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Application.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task<RoleDto> GetRoleByNameAsync(string name);
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task<RoleDto> UpdateRoleAsync(RoleDto roleDto);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    }
}
