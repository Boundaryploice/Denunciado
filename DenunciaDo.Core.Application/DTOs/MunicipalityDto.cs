namespace DenunciaDo.Core.Application.DTOs
{
    public class MunicipalityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<DistrictDto> Districts { get; set; } = new List<DistrictDto>();
    }
}
