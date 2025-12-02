using FreeLink.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace FreeLink.Infrastructure.Services;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly Client _supabaseClient;

    public SupabaseStorageService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"]
            ?? throw new InvalidOperationException("Supabase URL no configurada");
        var key = configuration["Supabase:Key"]
            ?? throw new InvalidOperationException("Supabase Key no configurada");

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false
        };

        _supabaseClient = new Client(url, key, options);
        _supabaseClient.InitializeAsync().Wait();
    }

    public async Task<string> UploadPdfAsync(byte[] fileBytes, string fileName, string bucketName, string folder = "")
    {
        return await UploadFileAsync(fileBytes, fileName, bucketName, folder, "application/pdf");
    }

    public async Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string bucketName, string folder = "", string? contentType = null)
    {
        try
        {
            // Construir ruta completa
            var filePath = string.IsNullOrEmpty(folder)
                ? fileName
                : $"{folder}/{fileName}";

            // Determinar content type si no se proporcionó
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = GetContentType(fileName);
            }

            // Subir archivo a Supabase Storage
            await _supabaseClient.Storage
                .From(bucketName)
                .Upload(fileBytes, filePath, new Supabase.Storage.FileOptions
                {
                    ContentType = contentType,
                    Upsert = true // Sobrescribir si existe
                });

            // Retornar URL p blica
            return GetPublicUrl(filePath, bucketName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al subir archivo a Supabase: {ex.Message}", ex);
        }
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    public string GetPublicUrl(string filePath, string bucketName)
    {
        return _supabaseClient.Storage
            .From(bucketName)
            .GetPublicUrl(filePath);
    }

    public async Task DeleteFileAsync(string filePath, string bucketName)
    {
        try
        {
            await _supabaseClient.Storage
                .From(bucketName)
                .Remove(new List<string> { filePath });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar archivo de Supabase: {ex.Message}", ex);
        }
    }
}