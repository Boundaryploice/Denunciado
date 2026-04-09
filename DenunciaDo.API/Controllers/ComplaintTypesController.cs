using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class ComplaintTypesController : BaseApiController
    {
        private readonly IComplaintTypeService _complaintTypeService;

        public ComplaintTypesController(IComplaintTypeService complaintTypeService)
        {
            _complaintTypeService = complaintTypeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ComplaintTypeDto>>> GetAllComplaintTypes()
        {
            var result = await _complaintTypeService.GetAllComplaintTypesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ComplaintTypeDto>> GetComplaintTypeById(int id)
        {
            var result = await _complaintTypeService.GetComplaintTypeByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Complaint type not found" });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ComplaintTypeDto>> CreateComplaintType(ComplaintTypeDto complaintTypeDto)
        {
            var result = await _complaintTypeService.CreateComplaintTypeAsync(complaintTypeDto);

            return CreatedAtAction(nameof(GetComplaintTypeById), new { id = result.Id }, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ComplaintTypeDto>> UpdateComplaintType(ComplaintTypeDto complaintTypeDto)
        {
            var result = await _complaintTypeService.UpdateComplaintTypeAsync(complaintTypeDto);

            if (result == null)
            {
                return BadRequest(new { message = "Complaint type update failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteComplaintType(int id)
        {
            var result = await _complaintTypeService.DeleteComplaintTypeAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "Complaint type deletion failed" });
            }

            return Ok(new { message = "Complaint type deleted successfully" });
        }
    }
}
