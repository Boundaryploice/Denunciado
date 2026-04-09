using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;

namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface ICommentService
    {
        Task<CommentDto> GetCommentByIdAsync(int id);
        Task<List<CommentDto>> GetCommentsByComplaintAsync(int complaintId);
        Task<List<CommentDto>> GetRepliesByCommentAsync(int commentId);
        Task<CommentDto> CreateCommentAsync(int userId, CreateCommentDto createDto);
        Task<CommentDto> UpdateCommentAsync(int id, int userId, string content);
        Task<bool> DeleteCommentAsync(int id, int userId);
    }
}
