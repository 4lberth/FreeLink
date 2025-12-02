using Microsoft.AspNetCore.Http;

namespace FreeLink.Domain.Ports;

public interface IFileService
{

    Task<string> SaveFileAsync(IFormFile file, string folder);

    Task<string> SaveFileAsync(IFormFile file, string folder, string bucketName);

    Task<bool> DeleteFileAsync(string filePath);

    bool ValidateFile(IFormFile file, string[] allowedExtensions, long maxSizeInBytes);

    string GetFileExtension(string fileName);
}
