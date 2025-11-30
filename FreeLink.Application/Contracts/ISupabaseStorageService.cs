namespace FreeLink.Application.Contracts;


public interface ISupabaseStorageService
{
    Task<string> UploadPdfAsync(byte[] fileBytes, string fileName, string folder = "");
    string GetPublicUrl(string filePath);
    Task DeleteFileAsync(string filePath);
}