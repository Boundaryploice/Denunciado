using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("upload-avatar")]
        public async Task<ActionResult> UploadAvatar(IFormFile avatar)
        {
            // TODO: Implementar subida de avatar
            return Ok(new { message = "Avatar upload - TODO: Implement" });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedListDto<UserDto>>> GetUsers(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetPaginatedUsersAsync(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = GetUserId();
            var result = await _userService.GetUserWithProfileAsync(userId);

            if (result == null)
            {
                return NotFound(new { message = "User profile not found" });
            }

            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto updateDto)
        {
            var userId = GetUserId();
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            if (result == null)
            {
                return BadRequest(new { message = "Profile update failed" });
            }

            return Ok(result);
        }

        [HttpGet("by-role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetUsersByRole(string role)
        {
            var result = await _userService.GetUsersByRoleAsync(role);
            return Ok(result);
        }

        [HttpPut("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            var result = await _userService.DeactivateUserAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "User deactivation failed" });
            }

            return Ok(new { message = "User deactivated successfully" });
        }

        [HttpPut("activate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateUser(int id)
        {
            var result = await _userService.ActivateUserAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "User activation failed" });
            }

            return Ok(new { message = "User activated successfully" });
        }
    }
}
