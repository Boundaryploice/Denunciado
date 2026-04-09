using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Interfaces.Repositories;

namespace DenunciaDo.Infrastructure.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IUserRepository Users { get; }
        public IComplaintRepository Complaints { get; }
        public IComplaintTypeRepository ComplaintTypes { get; }
        public IStatusRepository Statuses { get; }
        public IMunicipalityRepository Municipalities { get; }
        public IDistrictRepository Districts { get; }
        public IAttachmentRepository Attachments { get; }
        public IRoleRepository Roles { get; }
        public IUserRoleRepository UserRoles { get; }
        public IVoteRepository Votes { get; }
        public ICommentRepository Comments { get; }
        public IComplaintHistoryRepository ComplaintHistories { get; }
        public INotificationRepository Notifications { get; }
        public IUserProfileRepository UserProfiles { get; }

        public UnitOfWork(
            ApplicationDbContext dbContext,
            IUserRepository userRepository,
            IComplaintRepository complaintRepository,
            IComplaintTypeRepository complaintTypeRepository,
            IStatusRepository statusRepository,
            IMunicipalityRepository municipalityRepository,
            IDistrictRepository districtRepository,
            IAttachmentRepository attachmentRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IVoteRepository voteRepository,
            ICommentRepository commentRepository,
            IComplaintHistoryRepository complaintHistoryRepository,
            INotificationRepository notificationRepository,
            IUserProfileRepository userProfileRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Complaints = complaintRepository ?? throw new ArgumentNullException(nameof(complaintRepository));
            ComplaintTypes = complaintTypeRepository ?? throw new ArgumentNullException(nameof(complaintTypeRepository));
            Statuses = statusRepository ?? throw new ArgumentNullException(nameof(statusRepository));
            Municipalities = municipalityRepository ?? throw new ArgumentNullException(nameof(municipalityRepository));
            Districts = districtRepository ?? throw new ArgumentNullException(nameof(districtRepository));
            Attachments = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            Roles = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            UserRoles = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            Votes = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));
            Comments = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            ComplaintHistories = complaintHistoryRepository ?? throw new ArgumentNullException(nameof(complaintHistoryRepository));
            Notifications = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            UserProfiles = userProfileRepository ?? throw new ArgumentNullException(nameof(userProfileRepository));
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
