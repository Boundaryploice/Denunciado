namespace DenunciaDo.Core.Application.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
    }
}
