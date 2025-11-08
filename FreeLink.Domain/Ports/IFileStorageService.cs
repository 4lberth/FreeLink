namespace FreeLink.Domain.Ports;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);
    string GetFileUrl(string fileName, string folder);
}

