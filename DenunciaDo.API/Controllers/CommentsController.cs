using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class CommentsController : BaseApiController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommentDto>> GetCommentById(int id)
        {
            var result = await _commentService.GetCommentByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            return Ok(result);
        }

        [HttpGet("complaint/{complaintId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CommentDto>>> GetCommentsByComplaint(int complaintId)
        {
            var result = await _commentService.GetCommentsByComplaintAsync(complaintId);
            return Ok(result);
        }

        [HttpGet("replies/{commentId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CommentDto>>> GetRepliesByComment(int commentId)
        {
            var result = await _commentService.GetRepliesByCommentAsync(commentId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto createDto)
        {
            var userId = GetUserId();
            var result = await _commentService.CreateCommentAsync(userId, createDto);

            if (result == null)
            {
                return BadRequest(new { message = "Comment creation failed" });
            }

            return CreatedAtAction(nameof(GetCommentById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int id, [FromBody] string content)
        {
            var userId = GetUserId();
            var result = await _commentService.UpdateCommentAsync(id, userId, content);

            if (result == null)
            {
                return BadRequest(new { message = "Comment update failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var userId = GetUserId();
            var result = await _commentService.DeleteCommentAsync(id, userId);

            if (!result)
            {
                return BadRequest(new { message = "Comment deletion failed" });
            }

            return Ok(new { message = "Comment deleted successfully" });
        }
    }
}
