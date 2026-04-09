using DenunciaDo.Domain.Interfaces.Services;

namespace DenunciaDo.API.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _contentRootPath;

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _contentRootPath = _webHostEnvironment.ContentRootPath;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string containerName)
        {
            if (fileStream == null)
                return null;

            var extension = Path.GetExtension(fileName);
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var folder = Path.Combine(_contentRootPath, "wwwroot", containerName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filePath = Path.Combine(folder, newFileName);

            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            return $"{containerName}/{newFileName}";
        }

        public Task DeleteFileAsync(string fileRoute, string containerName)
        {
            if (string.IsNullOrEmpty(fileRoute))
                return Task.CompletedTask;

            var fileName = Path.GetFileName(fileRoute);
            var fileDirectory = Path.Combine(_contentRootPath, "wwwroot", containerName);
            var filePath = Path.Combine(fileDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        public string GetFileUrl(string fileName, string containerName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            return $"/{containerName}/{fileName}";
        }

        public async Task<Stream> GetFileStreamAsync(string fileRoute, string containerName)
        {
            var fileName = Path.GetFileName(fileRoute);
            var fileDirectory = Path.Combine(_contentRootPath, "wwwroot", containerName);
            var filePath = Path.Combine(fileDirectory, fileName);

            if (!File.Exists(filePath))
                return null;

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
