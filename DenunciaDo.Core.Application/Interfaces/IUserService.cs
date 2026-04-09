using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserWithProfileAsync(int id);
        Task<PaginatedListDto<UserDto>> GetPaginatedUsersAsync(int pageIndex, int pageSize);
        Task<List<UserDto>> GetUsersByRoleAsync(string role);
        Task<UserDto> UpdateUserAsync(int id, UpdateProfileDto updateDto);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileDto updateDto);
    }
}
