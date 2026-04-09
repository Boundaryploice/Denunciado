using AutoMapper;
using DenunciaDo.Application.DTOs;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Core.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenunciaDo.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public CommentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<CommentDto> GetCommentByIdAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);

            if (comment == null)
            {
                return null;
            }

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<List<CommentDto>> GetCommentsByComplaintAsync(int complaintId)
        {
            var comments = await _unitOfWork.Comments.GetCommentsByComplaintAsync(complaintId);

            // Get only top-level comments (no parents)
            var topLevelComments = comments.Where(c => c.ParentCommentId == null).ToList();

            var result = _mapper.Map<List<CommentDto>>(topLevelComments);

            // Get replies for each comment
            foreach (var commentDto in result)
            {
                var replies = await _unitOfWork.Comments.GetRepliesByCommentAsync(commentDto.Id);
                commentDto.Replies = _mapper.Map<List<CommentDto>>(replies);
            }

            return result;
        }

        public async Task<List<CommentDto>> GetRepliesByCommentAsync(int commentId)
        {
            var replies = await _unitOfWork.Comments.GetRepliesByCommentAsync(commentId);
            return _mapper.Map<List<CommentDto>>(replies);
        }

        public async Task<CommentDto> CreateCommentAsync(int userId, CreateCommentDto createDto)
        {
            // Validate complaint exists
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(createDto.ComplaintId);
            if (complaint == null)
            {
                return null;
            }

            // Validate parent comment if provided
            if (createDto.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.Comments.GetByIdAsync(createDto.ParentCommentId.Value);
                if (parentComment == null || parentComment.ComplaintId != createDto.ComplaintId)
                {
                    return null;
                }
            }

            // Create comment entity
            var comment = new Comment
            {
                Content = createDto.Content,
                UserId = userId,
                ComplaintId = createDto.ComplaintId,
                ParentCommentId = createDto.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Add comment to database
            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.CompleteAsync();

            // Notify the complaint owner if someone else commented
            if (complaint.UserId != userId)
            {
                await NotifyUserAboutComment(comment, complaint);
            }

            // Notify parent comment owner if this is a reply and not their own comment
            if (createDto.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.Comments.GetByIdAsync(createDto.ParentCommentId.Value);
                if (parentComment.UserId != userId)
                {
                    await NotifyUserAboutReply(comment, parentComment);
                }
            }

            // Return comment DTO
            var result = _mapper.Map<CommentDto>(comment);

            // Add user information
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            result.UserName = $"{user.FirstName} {user.LastName}";
            result.UserPicture = user.Picture;

            return result;
        }

        public async Task<CommentDto> UpdateCommentAsync(int id, int userId, string content)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);

            if (comment == null || comment.UserId != userId)
            {
                return null;
            }

            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Comments.UpdateAsync(comment);
            await _unitOfWork.CompleteAsync();

            // Return updated comment DTO
            var result = _mapper.Map<CommentDto>(comment);

            // Add user information
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            result.UserName = $"{user.FirstName} {user.LastName}";
            result.UserPicture = user.Picture;

            return result;
        }

        public async Task<bool> DeleteCommentAsync(int id, int userId)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);

            if (comment == null)
            {
                return false;
            }

            // Check if user owns the comment or has admin rights
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
            var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            bool isAdmin = userRoles.Contains("Admin");

            if (comment.UserId != userId && !isAdmin)
            {
                return false;
            }

            // Soft delete
            comment.IsActive = false;
            comment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Comments.UpdateAsync(comment);

            // Also soft delete all replies
            var replies = await _unitOfWork.Comments.GetRepliesByCommentAsync(id);
            foreach (var reply in replies)
            {
                reply.IsActive = false;
                reply.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Comments.UpdateAsync(reply);
            }

            await _unitOfWork.CompleteAsync();

            return true;
        }

        private async Task NotifyUserAboutComment(Comment comment, Complaint complaint)
        {
            var notification = new NotificationDto
            {
                UserId = complaint.UserId,
                Title = "New comment on your complaint",
                Message = $"Someone has commented on your complaint '{complaint.Title}'.",
                NotificationType = "NewComment",
                RelatedEntityType = "Complaint",
                RelatedEntityId = complaint.Id
            };

            await _notificationService.CreateNotificationAsync(notification);
        }

        private async Task NotifyUserAboutReply(Comment reply, Comment parentComment)
        {
            var notification = new NotificationDto
            {
                UserId = parentComment.UserId,
                Title = "New reply to your comment",
                Message = $"Someone has replied to your comment.",
                NotificationType = "CommentReply",
                RelatedEntityType = "Comment",
                RelatedEntityId = parentComment.Id
            };

            await _notificationService.CreateNotificationAsync(notification);
        }
    }
}
