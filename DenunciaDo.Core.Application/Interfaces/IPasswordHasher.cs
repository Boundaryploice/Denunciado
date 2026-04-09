namespace DenunciaDo.Core.Domain.Interfaces.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
