using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface IVoteService
    {
        Task<VoteDto> VoteAsync(int userId, VoteRequestDto voteRequest);
        Task<bool> DeleteVoteAsync(int userId, int complaintId);
        Task<VoteDto> GetVoteByUserAndComplaintAsync(int userId, int complaintId);
    }
}
