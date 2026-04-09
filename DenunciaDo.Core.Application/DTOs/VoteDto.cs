namespace DenunciaDo.Core.Application.DTOs
{
    public class VoteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ComplaintId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
