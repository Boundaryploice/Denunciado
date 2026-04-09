namespace DenunciaDo.Core.Application.DTOs
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Image { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPicture { get; set; }
        public int ComplaintTypeId { get; set; }
        public string ComplaintTypeName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string MunicipalityName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int CommentCount { get; set; }
        public bool HasUserVoted { get; set; }
        public bool? UserVoteType { get; set; }
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
    }
}
