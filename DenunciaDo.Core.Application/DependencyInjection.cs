using DenunciaDo.Application.Interfaces;
using DenunciaDo.Application.Services;
using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DenunciaDo.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<IComplaintTypeService, ComplaintTypeService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IMunicipalityService, MunicipalityService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}