using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserDto> RegisterAsync(RegisterRequestDto request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request);
    }
}
