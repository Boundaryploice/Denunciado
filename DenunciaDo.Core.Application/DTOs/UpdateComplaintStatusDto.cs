namespace DenunciaDo.Application.DTOs
{
    public class UpdateComplaintStatusDto
    {
        public int ComplaintId { get; set; }
        public int StatusId { get; set; }
        public string Comments { get; set; }
    }
}
