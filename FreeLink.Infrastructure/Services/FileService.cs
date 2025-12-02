using FreeLink.Domain.Ports;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace FreeLink.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly ISupabaseStorageService _supabaseStorageService;
    private const string DefaultBucket = "project-files";

    public FileService(ISupabaseStorageService supabaseStorageService)
    {
        _supabaseStorageService = supabaseStorageService;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        return await SaveFileAsync(file, folder, DefaultBucket);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder, string bucketName)
    {
        try
        {
            // Generar nombre único para el archivo
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            // Convertir IFormFile a byte array
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Subir a Supabase Storage
            var publicUrl = await _supabaseStorageService.UploadFileAsync(
                fileBytes,
                uniqueFileName,
                bucketName,
                folder
            );

            return publicUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al guardar archivo: {ex.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            await _supabaseStorageService.DeleteFileAsync(filePath, DefaultBucket);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool ValidateFile(IFormFile file, string[] allowedExtensions, long maxSizeInBytes)
    {
        if (file == null || file.Length == 0)
        {
            return false;
        }

        // Validar tamaño
        if (file.Length > maxSizeInBytes)
        {
            return false;
        }

        // Validar extensión
        var extension = GetFileExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return false;
        }

        return true;
    }

    public string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).TrimStart('.');
    }
}