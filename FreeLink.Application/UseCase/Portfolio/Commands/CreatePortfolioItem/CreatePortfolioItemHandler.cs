using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.CreatePortfolioItem;

public class CreatePortfolioItemHandler : IRequestHandler<CreatePortfolioItemCommand, CreatePortfolioItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePortfolioItemHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePortfolioItemResponse> Handle(CreatePortfolioItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el usuario existe
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new CreatePortfolioItemResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Validar que solo el dueño puede agregar a su portafolio
            if (request.RequestingUserId != request.UserId)
            {
                return new CreatePortfolioItemResponse
                {
                    Success = false,
                    Message = "No tienes permiso para agregar items a este portafolio"
                };
            }

            var portfolioItem = new Portfolioitem
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
                ProjectUrl = request.ProjectUrl,
                ThumbnailUrl = request.ThumbnailUrl,
                CompletionDate = request.CompletionDate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Portfolioitem>().Add(portfolioItem);
            await _unitOfWork.Complete();

            return new CreatePortfolioItemResponse
            {
                Success = true,
                Message = "Item agregado al portafolio exitosamente",
                PortfolioId = portfolioItem.PortfolioId
            };
        }
        catch (Exception ex)
        {
            return new CreatePortfolioItemResponse
            {
                Success = false,
                Message = $"Error al crear item de portafolio: {ex.Message}"
            };
        }
    }
}
