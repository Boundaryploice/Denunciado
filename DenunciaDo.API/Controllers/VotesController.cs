using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class VotesController : BaseApiController
    {
        private readonly IVoteService _voteService;

        public VotesController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        [HttpGet("{complaintId}")]
        public async Task<ActionResult<VoteDto>> GetUserVote(int complaintId)
        {
            var userId = GetUserId();
            var result = await _voteService.GetVoteByUserAndComplaintAsync(userId, complaintId);

            if (result == null)
            {
                return NotFound(new { message = "Vote not found" });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<VoteDto>> Vote(VoteRequestDto voteRequest)
        {
            var userId = GetUserId();
            var result = await _voteService.VoteAsync(userId, voteRequest);

            if (result == null)
            {
                return BadRequest(new { message = "Vote failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{complaintId}")]
        public async Task<ActionResult> DeleteVote(int complaintId)
        {
            var userId = GetUserId();
            var result = await _voteService.DeleteVoteAsync(userId, complaintId);

            if (!result)
            {
                return BadRequest(new { message = "Vote deletion failed" });
            }

            return Ok(new { message = "Vote deleted successfully" });
        }
    }

}
