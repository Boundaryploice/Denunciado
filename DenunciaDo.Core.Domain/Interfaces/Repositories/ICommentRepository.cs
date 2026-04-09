using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;

namespace DenunciaDo.Domain.Interfaces.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IReadOnlyList<Comment>> GetCommentsByUserAsync(int userId);
        Task<IReadOnlyList<Comment>> GetCommentsByComplaintAsync(int complaintId);
        Task<IReadOnlyList<Comment>> GetRepliesByCommentAsync(int commentId);
    }
}
