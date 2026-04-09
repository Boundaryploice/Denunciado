namespace DenunciaDo.Application.DTOs
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public int ComplaintId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
