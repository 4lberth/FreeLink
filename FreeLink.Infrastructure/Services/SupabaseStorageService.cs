using FreeLink.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace FreeLink.Infrastructure.Services;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly Client _supabaseClient;
    private readonly string _bucketName;

    public SupabaseStorageService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"] 
            ?? throw new InvalidOperationException("Supabase URL no configurada");
        var key = configuration["Supabase:Key"] 
            ?? throw new InvalidOperationException("Supabase Key no configurada");
        _bucketName = configuration["Supabase:BucketName"] ?? "contracts";

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false
        };

        _supabaseClient = new Client(url, key, options);
        _supabaseClient.InitializeAsync().Wait();
    }

    public async Task<string> UploadPdfAsync(byte[] fileBytes, string fileName, string folder = "")
    {
        try
        {
            // Construir ruta completa
            var filePath = string.IsNullOrEmpty(folder) 
                ? fileName 
                : $"{folder}/{fileName}";

            // Subir archivo a Supabase Storage
            await _supabaseClient.Storage
                .From(_bucketName)
                .Upload(fileBytes, filePath, new Supabase.Storage.FileOptions
                {
                    ContentType = "application/pdf",
                    Upsert = false // No sobrescribir si existe
                });

            // Retornar URL pública
            return GetPublicUrl(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al subir PDF a Supabase: {ex.Message}", ex);
        }
    }

    public string GetPublicUrl(string filePath)
    {
        return _supabaseClient.Storage
            .From(_bucketName)
            .GetPublicUrl(filePath);
    }

    public async Task DeleteFileAsync(string filePath)
    {
        try
        {
            await _supabaseClient.Storage
                .From(_bucketName)
                .Remove(new List<string> { filePath });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar archivo de Supabase: {ex.Message}", ex);
        }
    }
}