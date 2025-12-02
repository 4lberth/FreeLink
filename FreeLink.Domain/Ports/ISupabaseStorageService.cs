namespace FreeLink.Domain.Ports;


public interface ISupabaseStorageService
{
    Task<string> UploadPdfAsync(byte[] fileBytes, string fileName, string bucketName, string folder = "");
    Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string bucketName, string folder = "", string? contentType = null);
    string GetPublicUrl(string filePath, string bucketName);
    Task DeleteFileAsync(string filePath, string bucketName);
}
