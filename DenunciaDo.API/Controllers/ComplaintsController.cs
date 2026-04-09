using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class ComplaintsController : BaseApiController
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }
        [HttpGet("map-data")]
        [AllowAnonymous]
        public async Task<ActionResult> GetComplaintsForMap(
    [FromQuery] decimal? lat,
    [FromQuery] decimal? lng,
    [FromQuery] decimal? radius)
        {
            // TODO: Implementar datos para mapa
            return Ok(new { message = "Map data endpoint - TODO: Implement" });
        }

        [HttpGet("export")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> ExportComplaints([FromQuery] string format = "csv")
        {
            // TODO: Implementar exportación
            return Ok(new { message = "Export endpoint - TODO: Implement" });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedListDto<ComplaintDto>>> GetComplaints(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetPaginatedComplaintsAsync(pageIndex, pageSize, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ComplaintDetailDto>> GetComplaintById(int id)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetComplaintByIdAsync(id, userId);

            if (result == null)
            {
                return NotFound(new { message = "Complaint not found" });
            }

            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<ActionResult<PaginatedListDto<ComplaintDto>>> GetUserComplaints(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var result = await _complaintService.GetComplaintsByUserAsync(userId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("type/{typeId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedListDto<ComplaintDto>>> GetComplaintsByType(
            int typeId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetComplaintsByTypeAsync(typeId, pageIndex, pageSize, userId);
            return Ok(result);
        }

        [HttpGet("status/{statusId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedListDto<ComplaintDto>>> GetComplaintsByStatus(
            int statusId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetComplaintsByStatusAsync(statusId, pageIndex, pageSize, userId);
            return Ok(result);
        }

        [HttpGet("district/{districtId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedListDto<ComplaintDto>>> GetComplaintsByDistrict(
            int districtId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetComplaintsByDistrictAsync(districtId, pageIndex, pageSize, userId);
            return Ok(result);
        }

        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ComplaintDto>>> GetRecentComplaints([FromQuery] int count = 5)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetRecentComplaintsAsync(count, userId);
            return Ok(result);
        }

        [HttpGet("top-voted")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ComplaintDto>>> GetTopVotedComplaints([FromQuery] int count = 5)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.GetTopVotedComplaintsAsync(count, userId);
            return Ok(result);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedListDto<ComplaintDto>>> SearchComplaints(
            [FromQuery] string term,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
            var result = await _complaintService.SearchComplaintsAsync(term, pageIndex, pageSize, userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ComplaintDto>> CreateComplaint([FromForm] CreateComplaintDto createDto)
        {
            var userId = GetUserId();
            var result = await _complaintService.CreateComplaintAsync(userId, createDto);

            return CreatedAtAction(nameof(GetComplaintById), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<ActionResult<ComplaintDto>> UpdateComplaint(UpdateComplaintDto updateDto)
        {
            var userId = GetUserId();
            var result = await _complaintService.UpdateComplaintAsync(userId, updateDto);

            if (result == null)
            {
                return BadRequest(new { message = "Complaint update failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComplaint(int id)
        {
            var userId = GetUserId();
            var result = await _complaintService.DeleteComplaintAsync(id, userId);

            if (!result)
            {
                return BadRequest(new { message = "Complaint deletion failed" });
            }

            return Ok(new { message = "Complaint deleted successfully" });
        }

        [HttpPut("status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ComplaintDto>> UpdateComplaintStatus(UpdateComplaintStatusDto updateDto)
        {
            var userId = GetUserId();
            var result = await _complaintService.UpdateComplaintStatusAsync(userId, updateDto);

            if (result == null)
            {
                return BadRequest(new { message = "Complaint status update failed" });
            }

            return Ok(result);
        }
    }
}
