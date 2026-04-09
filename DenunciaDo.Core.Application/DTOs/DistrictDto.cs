namespace DenunciaDo.Core.Application.DTOs
{
    public class DistrictDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
    }
}
