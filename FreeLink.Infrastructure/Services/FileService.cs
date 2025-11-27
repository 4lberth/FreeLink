using FreeLink.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace FreeLink.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IHostEnvironment _environment;

    public FileService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        try
        {
            // Crear carpeta si no existe
            var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generar nombre único para el archivo
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retornar ruta relativa
            return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
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
            var fullPath = Path.Combine(_environment.ContentRootPath, "uploads", filePath);
            
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                return true;
            }

            return false;
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
