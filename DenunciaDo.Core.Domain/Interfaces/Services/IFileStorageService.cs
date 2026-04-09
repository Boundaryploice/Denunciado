namespace DenunciaDo.Domain.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string containerName);
        Task DeleteFileAsync(string fileRoute, string containerName);
        string GetFileUrl(string fileName, string containerName);
        Task<Stream> GetFileStreamAsync(string fileRoute, string containerName);
    }
}
