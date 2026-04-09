using AutoMapper;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
            CreateMap<UserProfile, UserProfileDto>();

            // Complaint mappings
            CreateMap<Complaint, ComplaintDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserPicture, opt => opt.MapFrom(src => src.User.Picture))
                .ForMember(dest => dest.ComplaintTypeName, opt => opt.MapFrom(src => src.ComplaintType.Name))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => src.Status.Color))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.District != null ? src.District.Name : null))
                .ForMember(dest => dest.MunicipalityName, opt => opt.MapFrom(src => src.District != null && src.District.Municipality != null ? src.District.Municipality.Name : null))
                .ForMember(dest => dest.UpvoteCount, opt => opt.Ignore())
                .ForMember(dest => dest.DownvoteCount, opt => opt.Ignore())
                .ForMember(dest => dest.CommentCount, opt => opt.Ignore())
                .ForMember(dest => dest.HasUserVoted, opt => opt.Ignore())
                .ForMember(dest => dest.UserVoteType, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore());

            CreateMap<Complaint, ComplaintDetailDto>();

            // ComplaintType mappings
            CreateMap<ComplaintType, ComplaintTypeDto>();
            CreateMap<ComplaintTypeDto, ComplaintType>();

            // Status mappings
            CreateMap<Status, StatusDto>();
            CreateMap<StatusDto, Status>();

            // Municipality mappings
            CreateMap<Municipality, MunicipalityDto>()
                .ForMember(dest => dest.Districts, opt => opt.Ignore());
            CreateMap<MunicipalityDto, Municipality>();

            // District mappings
            CreateMap<District, DistrictDto>()
                .ForMember(dest => dest.MunicipalityName, opt => opt.MapFrom(src => src.Municipality.Name));
            CreateMap<DistrictDto, District>();

            // Vote mappings
            CreateMap<Vote, VoteDto>();
            CreateMap<VoteDto, Vote>();

            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName, 
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserPicture, opt => opt.MapFrom(src => src.User.Picture))
                .ForMember(dest => dest.Replies, opt => opt.Ignore());

            // Attachment mappings
            CreateMap<Attachment, AttachmentDto>();

            // ComplaintHistory mappings
            CreateMap<ComplaintHistory, ComplaintHistoryDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : "System"))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => src.Status.Color));

            // Notification mappings
            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationDto, Notification>();
        }
    }
}