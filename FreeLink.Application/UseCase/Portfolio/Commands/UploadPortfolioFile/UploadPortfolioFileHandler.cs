using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.UploadPortfolioFile;

public class UploadPortfolioFileHandler : IRequestHandler<UploadPortfolioFileCommand, UploadPortfolioFileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private static readonly string[] AllowedExtensions = { "jpg", "jpeg", "png", "gif", "mp4", "pdf", "doc", "docx" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public UploadPortfolioFileHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<UploadPortfolioFileResponse> Handle(UploadPortfolioFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el item de portafolio existe
            var portfolioItem = await _unitOfWork.Repository<Portfolioitem>().GetById(request.PortfolioId);
            if (portfolioItem == null)
            {
                return new UploadPortfolioFileResponse
                {
                    Success = false,
                    Message = "Item de portafolio no encontrado"
                };
            }

            // Verificar que el usuario es el dueño
            if (portfolioItem.UserId != request.RequestingUserId)
            {
                return new UploadPortfolioFileResponse
                {
                    Success = false,
                    Message = "No tienes permiso para subir archivos a este item"
                };
            }

            // Validar archivo
            if (!_fileService.ValidateFile(request.File, AllowedExtensions, MaxFileSize))
            {
                return new UploadPortfolioFileResponse
                {
                    Success = false,
                    Message = "Archivo inválido. Verifica el tipo y tamaño (máx 10MB)"
                };
            }

            // Guardar archivo
            var filePath = await _fileService.SaveFileAsync(request.File, $"portfolio/{portfolioItem.UserId}");

            // Crear registro en BD
            var portfolioFile = new Portfoliofile
            {
                PortfolioId = request.PortfolioId,
                FileName = request.File.FileName,  // ← AGREGADO
                FileUrl = filePath,
                FileType = request.FileType,
                FileSize = request.File.Length,
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Portfoliofile>().Add(portfolioFile);
            await _unitOfWork.Complete();

            return new UploadPortfolioFileResponse
            {
                Success = true,
                Message = "Archivo subido exitosamente",
                FileId = portfolioFile.FileId,
                FileUrl = filePath
            };
        }
        catch (Exception ex)
        {
            return new UploadPortfolioFileResponse
            {
                Success = false,
                Message = $"Error al subir archivo: {ex.Message}"
            };
        }
    }
}
