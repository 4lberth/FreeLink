using System.IO;


namespace FreeLink.Application.Services{
    /// <summary>
    /// Representa un archivo a subir, desacoplado de IFormFile para
    /// poder usarse desde Application/Infrastructure sin referenciar ASP.NET.
    /// </summary>
    public sealed class FileUploadRequest
    {
        public Stream Stream { get; init; } = Stream.Null;
        public string FileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = "application/octet-stream";
        public long Length { get; init; }
    }
}