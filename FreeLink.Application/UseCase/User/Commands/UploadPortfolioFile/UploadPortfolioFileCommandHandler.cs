using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UploadPortfolioFile;

public class UploadPortfolioFileCommandHandler : IRequestHandler<UploadPortfolioFileCommand, UploadPortfolioFileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public UploadPortfolioFileCommandHandler(
        IUnitOfWork unitOfWork, 
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<UploadPortfolioFileResponse> Handle(UploadPortfolioFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el usuario existe
            var user = await _unitOfWork.Repository<User>().GetById(request.UserId);
            if (user == null)
            {
                return new UploadPortfolioFileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            var fileUrls = new List<string>();
            string? thumbnailUrl = null;

            // 2. Guardar thumbnail si existe
            if (request.ThumbnailFile != null && request.ThumbnailFile.Length > 0)
            {
                var thumbnailFileName = await _fileStorageService.SaveFileAsync(
                    request.ThumbnailFile.OpenReadStream(), 
                    request.ThumbnailFile.FileName, 
                    "portfolio");
                
                thumbnailUrl = _fileStorageService.GetFileUrl(thumbnailFileName, "portfolio");
            }

            // 4. Crear item de portafolio
            var portfolioItem = new Portfolioitem
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
                ProjectUrl = request.ProjectUrl,
                ThumbnailUrl = thumbnailUrl,
                CompletionDate = request.CompletionDate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Portfolioitem>().Add(portfolioItem);
            await _unitOfWork.Complete();

            // 5. Guardar archivos del portafolio
            if (request.Files != null && request.Files.Any())
            {
                foreach (var file in request.Files)
                {
                    if (file.Length > 0)
                    {
                        var savedFileName = await _fileStorageService.SaveFileAsync(
                            file.OpenReadStream(), 
                            file.FileName, 
                            "portfolio");
                        
                        var fileUrl = _fileStorageService.GetFileUrl(savedFileName, "portfolio");

                        var portfolioFile = new Portfoliofile
                        {
                            PortfolioId = portfolioItem.PortfolioId,
                            FileName = file.FileName,
                            FileUrl = fileUrl,
                            FileType = file.ContentType,
                            FileSize = file.Length,
                            UploadedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.Repository<Portfoliofile>().Add(portfolioFile);
                        fileUrls.Add(fileUrl);
                    }
                }
            }

            await _unitOfWork.Complete();

            return new UploadPortfolioFileResponse
            {
                Success = true,
                Message = "Archivos de portafolio subidos exitosamente",
                PortfolioId = portfolioItem.PortfolioId,
                FileUrls = fileUrls
            };
        }
        catch (Exception ex)
        {
            return new UploadPortfolioFileResponse
            {
                Success = false,
                Message = $"Error al subir archivos: {ex.Message}"
            };
        }
    }
}

