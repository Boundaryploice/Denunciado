namespace DenunciaDo.Application.DTOs
{
    public class VoteRequestDto
    {
        public int ComplaintId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
