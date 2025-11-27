using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.UpdatePortfolioItem;

public class UpdatePortfolioItemHandler : IRequestHandler<UpdatePortfolioItemCommand, UpdatePortfolioItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePortfolioItemHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePortfolioItemResponse> Handle(UpdatePortfolioItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _unitOfWork.Repository<Portfolioitem>().GetById(request.PortfolioId);
            if (item == null)
            {
                return new UpdatePortfolioItemResponse
                {
                    Success = false,
                    Message = "Item de portafolio no encontrado"
                };
            }

            if (item.UserId != request.RequestingUserId)
            {
                return new UpdatePortfolioItemResponse
                {
                    Success = false,
                    Message = "No tienes permiso para actualizar este item"
                };
            }

            item.Title = request.Title;
            item.Description = request.Description;
            item.ProjectUrl = request.ProjectUrl;
            item.ThumbnailUrl = request.ThumbnailUrl;
            item.CompletionDate = request.CompletionDate;

            await _unitOfWork.Repository<Portfolioitem>().Update(item);
            await _unitOfWork.Complete();

            return new UpdatePortfolioItemResponse
            {
                Success = true,
                Message = "Item actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdatePortfolioItemResponse
            {
                Success = false,
                Message = $"Error al actualizar item: {ex.Message}"
            };
        }
    }
}
