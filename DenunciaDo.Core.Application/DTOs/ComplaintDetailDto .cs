namespace DenunciaDo.Core.Application.DTOs
{
    public class ComplaintDetailDto : ComplaintDto
    {
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public List<ComplaintHistoryDto> History { get; set; } = new List<ComplaintHistoryDto>();
    }
}
