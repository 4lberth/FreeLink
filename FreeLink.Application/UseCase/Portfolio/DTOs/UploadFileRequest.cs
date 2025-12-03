using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.Portfolio.DTOs;

/// Request para subir archivo al portafolio
public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public string FileType { get; set; } = string.Empty;
}