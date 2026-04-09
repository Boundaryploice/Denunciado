namespace DenunciaDo.Core.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string Picture { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public List<string> Roles { get; set; } = new List<string>();
    }
}
