using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class StatusesController : BaseApiController
    {
        private readonly IStatusService _statusService;

        public StatusesController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<StatusDto>>> GetAllStatuses()
        {
            var result = await _statusService.GetAllStatusesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<StatusDto>> GetStatusById(int id)
        {
            var result = await _statusService.GetStatusByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Status not found" });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StatusDto>> CreateStatus(StatusDto statusDto)
        {
            var result = await _statusService.CreateStatusAsync(statusDto);

            return CreatedAtAction(nameof(GetStatusById), new { id = result.Id }, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StatusDto>> UpdateStatus(StatusDto statusDto)
        {
            var result = await _statusService.UpdateStatusAsync(statusDto);

            if (result == null)
            {
                return BadRequest(new { message = "Status update failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteStatus(int id)
        {
            var result = await _statusService.DeleteStatusAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "Status deletion failed" });
            }

            return Ok(new { message = "Status deleted successfully" });
        }
    }
}
