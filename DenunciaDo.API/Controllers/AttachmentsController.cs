using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttachmentsController : BaseApiController
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AttachmentDto>> GetAttachment(int id)
        {
            var result = await _attachmentService.GetAttachmentByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("complaint/{complaintId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AttachmentDto>>> GetAttachmentsByComplaint(int complaintId)
        {
            var result = await _attachmentService.GetAttachmentsByComplaintAsync(complaintId);
            return Ok(result);
        }

        [HttpPost("complaint/{complaintId}")]
        public async Task<ActionResult<List<AttachmentDto>>> UploadAttachments(int complaintId, IFormFileCollection files)
        {
            var result = await _attachmentService.UploadAttachmentsAsync(complaintId, files);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAttachment(int id)
        {
            var result = await _attachmentService.DeleteAttachmentAsync(id);
            if (!result)
                return BadRequest(new { message = "Failed to delete attachment" });
            return Ok(new { message = "Attachment deleted successfully" });
        }
    }
}
