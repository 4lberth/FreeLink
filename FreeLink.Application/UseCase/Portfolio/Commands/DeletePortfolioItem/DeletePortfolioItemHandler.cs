
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.DeletePortfolioItem;

public class DeletePortfolioItemHandler : IRequestHandler<DeletePortfolioItemCommand, DeletePortfolioItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public DeletePortfolioItemHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<DeletePortfolioItemResponse> Handle(DeletePortfolioItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _unitOfWork.Repository<Portfolioitem>().GetById(request.PortfolioId);
            if (item == null)
            {
                return new DeletePortfolioItemResponse
                {
                    Success = false,
                    Message = "Item de portafolio no encontrado"
                };
            }

            if (item.UserId != request.RequestingUserId)
            {
                return new DeletePortfolioItemResponse
                {
                    Success = false,
                    Message = "No tienes permiso para eliminar este item"
                };
            }

            // Obtener archivos asociados
            var files = await _unitOfWork.Repository<Portfoliofile>()
                .GetAsync(f => f.PortfolioId == request.PortfolioId);

            // Eliminar archivos físicos y registros
            foreach (var file in files)
            {
                await _fileService.DeleteFileAsync(file.FileUrl);
                await _unitOfWork.Repository<Portfoliofile>().Delete(file.FileId);
            }

            // Eliminar item
            await _unitOfWork.Repository<Portfolioitem>().Delete(request.PortfolioId);
            await _unitOfWork.Complete();

            return new DeletePortfolioItemResponse
            {
                Success = true,
                Message = "Item y archivos eliminados exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DeletePortfolioItemResponse
            {
                Success = false,
                Message = $"Error al eliminar item: {ex.Message}"
            };
        }
    }
}
