using DenunciaDo.Application.DTOs;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
          
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(RegisterRequestDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest(new { message = "User registration failed" });
            }

            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = GetUserId();
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result)
            {
                return BadRequest(new { message = "Password change failed" });
            }

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
