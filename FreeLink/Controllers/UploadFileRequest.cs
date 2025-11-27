namespace FreeLink.Controllers;

/// <summary>
/// Request para subir archivo al portafolio
/// </summary>
public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public string FileType { get; set; } = string.Empty;
}
