namespace DenunciaDo.Core.Application.DTOs
{
    public class ComplaintHistoryDto
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
