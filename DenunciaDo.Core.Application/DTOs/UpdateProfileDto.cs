namespace DenunciaDo.Application.DTOs
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
