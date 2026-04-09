using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(int id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto roleDto)
        {
            var result = await _roleService.CreateRoleAsync(roleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<ActionResult<RoleDto>> UpdateRole(RoleDto roleDto)
        {
            var result = await _roleService.UpdateRoleAsync(roleDto);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpPost("assign/{userId}/{roleId}")]
        public async Task<ActionResult> AssignRoleToUser(int userId, int roleId)
        {
            var result = await _roleService.AssignRoleToUserAsync(userId, roleId);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("remove/{userId}/{roleId}")]
        public async Task<ActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            if (!result)
                return BadRequest();
            return Ok();
        }
    }
}
