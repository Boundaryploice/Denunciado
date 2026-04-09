namespace DenunciaDo.Core.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
