namespace DenunciaDo.Core.Application.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public UserDto User { get; set; }
        public string Message { get; set; }
    }
}
