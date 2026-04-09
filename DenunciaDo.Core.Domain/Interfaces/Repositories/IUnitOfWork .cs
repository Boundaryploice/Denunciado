using DenunciaDo.Domain.Interfaces.Repositories;

namespace DenunciaDo.Core.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IComplaintRepository Complaints { get; }
        IComplaintTypeRepository ComplaintTypes { get; }
        IStatusRepository Statuses { get; }
        IMunicipalityRepository Municipalities { get; }
        IDistrictRepository Districts { get; }
        IAttachmentRepository Attachments { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        IVoteRepository Votes { get; }
        ICommentRepository Comments { get; }
        IComplaintHistoryRepository ComplaintHistories { get; }
        INotificationRepository Notifications { get; }
        IUserProfileRepository UserProfiles { get; }

        Task<int> CompleteAsync();
    }
}
