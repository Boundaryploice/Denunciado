using DenunciaDo.Core.Application.Interfaces;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Interfaces.Services;
using DenunciaDo.Infrastructure.Data;
using DenunciaDo.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DenunciaDo.Infrastructure
{ 
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IComplaintTypeRepository, ComplaintTypeRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<IMunicipalityRepository, MunicipalityRepository>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IComplaintHistoryRepository, ComplaintHistoryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            //services.AddScoped<ITokenService, TokenService>();
            //services.AddScoped<IFileStorageService, FileStorageService>(); // Interface del dominio
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = System.TimeSpan.Zero
                    };
                });

            return services;
        }
    }

}