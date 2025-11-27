using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.Contracts;

public interface IFileService
{

    Task<string> SaveFileAsync(IFormFile file, string folder);

    Task<bool> DeleteFileAsync(string filePath);

    bool ValidateFile(IFormFile file, string[] allowedExtensions, long maxSizeInBytes);

    string GetFileExtension(string fileName);
}
