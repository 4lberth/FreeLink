
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.DeletePortfolioFile;

public class DeletePortfolioFileHandler : IRequestHandler<DeletePortfolioFileCommand, DeletePortfolioFileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public DeletePortfolioFileHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<DeletePortfolioFileResponse> Handle(DeletePortfolioFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener archivo
            var file = await _unitOfWork.Repository<Portfoliofile>().GetById(request.FileId);
            if (file == null)
            {
                return new DeletePortfolioFileResponse
                {
                    Success = false,
                    Message = "Archivo no encontrado"
                };
            }

            // Verificar permisos (obtener item de portafolio)
            var portfolioItem = await _unitOfWork.Repository<Portfolioitem>().GetById(file.PortfolioId);
            if (portfolioItem != null && portfolioItem.UserId != request.RequestingUserId)
            {
                return new DeletePortfolioFileResponse
                {
                    Success = false,
                    Message = "No tienes permiso para eliminar este archivo"
                };
            }

            // Eliminar archivo físico
            await _fileService.DeleteFileAsync(file.FileUrl);

            // Eliminar registro
            await _unitOfWork.Repository<Portfoliofile>().Delete(request.FileId);
            await _unitOfWork.Complete();

            return new DeletePortfolioFileResponse
            {
                Success = true,
                Message = "Archivo eliminado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DeletePortfolioFileResponse
            {
                Success = false,
                Message = $"Error al eliminar archivo: {ex.Message}"
            };
        }
    }
}
