using AutoMapper;
using DenunciaDo.Application.DTOs;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Core.Domain.Interfaces.Services;

namespace DenunciaDo.Application.Services
{
    public class VoteService : IVoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public VoteService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<VoteDto> VoteAsync(int userId, VoteRequestDto voteRequest)
        {
            // Check if complaint exists
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(voteRequest.ComplaintId);
            if (complaint == null)
            {
                return null;
            }

            // Check if user has already voted
            var existingVote = await _unitOfWork.Votes.GetVoteByUserAndComplaintAsync(userId, voteRequest.ComplaintId);

            if (existingVote != null)
            {
                // Update existing vote if different
                if (existingVote.IsUpvote != voteRequest.IsUpvote)
                {
                    existingVote.IsUpvote = voteRequest.IsUpvote;
                    existingVote.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Votes.UpdateAsync(existingVote);
                    await _unitOfWork.CompleteAsync();
                }

                return _mapper.Map<VoteDto>(existingVote);
            }

            // Create new vote
            var vote = new Vote
            {
                UserId = userId,
                ComplaintId = voteRequest.ComplaintId,
                IsUpvote = voteRequest.IsUpvote,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Votes.AddAsync(vote);
            await _unitOfWork.CompleteAsync();

            // Notify complaint owner if someone else voted on their complaint
            if (complaint.UserId != userId)
            {
                await NotifyUserAboutVote(vote, complaint);
            }

            return _mapper.Map<VoteDto>(vote);
        }

        public async Task<bool> DeleteVoteAsync(int userId, int complaintId)
        {
            var vote = await _unitOfWork.Votes.GetVoteByUserAndComplaintAsync(userId, complaintId);

            if (vote == null)
            {
                return false;
            }

            await _unitOfWork.Votes.DeleteAsync(vote);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<VoteDto> GetVoteByUserAndComplaintAsync(int userId, int complaintId)
        {
            var vote = await _unitOfWork.Votes.GetVoteByUserAndComplaintAsync(userId, complaintId);

            if (vote == null)
            {
                return null;
            }

            return _mapper.Map<VoteDto>(vote);
        }

        private async Task NotifyUserAboutVote(Vote vote, Complaint complaint)
        {
            var voteType = vote.IsUpvote ? "upvoted" : "downvoted";

            var notification = new NotificationDto
            {
                UserId = complaint.UserId,
                Title = $"Someone {voteType} your complaint",
                Message = $"Someone has {voteType} your complaint '{complaint.Title}'.",
                NotificationType = "Vote",
                RelatedEntityType = "Complaint",
                RelatedEntityId = complaint.Id
            };

            await _notificationService.CreateNotificationAsync(notification);
        }
    }
}
