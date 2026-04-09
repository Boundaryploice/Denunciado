namespace DenunciaDo.Application.DTOs
{
    public class UpdateComplaintDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ComplaintTypeId { get; set; }
        public int? DistrictId { get; set; }
    }
}
