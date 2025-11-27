using FreeLink.Application.UseCase.Portfolio.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Queries.GetPortfolioItemById;

public class GetPortfolioItemByIdHandler : IRequestHandler<GetPortfolioItemByIdQuery, GetPortfolioItemByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPortfolioItemByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPortfolioItemByIdResponse> Handle(GetPortfolioItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _unitOfWork.Repository<Portfolioitem>().GetById(request.PortfolioId);
            if (item == null)
            {
                return new GetPortfolioItemByIdResponse
                {
                    Success = false,
                    Message = "Item de portafolio no encontrado"
                };
            }

            // Obtener archivos
            var files = await _unitOfWork.Repository<Portfoliofile>()
                .GetAsync(f => f.PortfolioId == request.PortfolioId);

            var fileDtos = files.Select(f => new PortfolioFileDto
            {
                FileId = f.FileId,
                PortfolioId = f.PortfolioId,
                FileUrl = f.FileUrl,
                FileType = f.FileType,
                FileSize = f.FileSize.HasValue ? f.FileSize.Value : 0,
                UploadedAt = f.UploadedAt
            }).ToList();

            var itemDto = new PortfolioItemDto
            {
                PortfolioId = item.PortfolioId,
                UserId = item.UserId,
                Title = item.Title,
                Description = item.Description,
                ProjectUrl = item.ProjectUrl,
                ThumbnailUrl = item.ThumbnailUrl,
                CompletionDate = item.CompletionDate,
                CreatedAt = item.CreatedAt,
                Files = fileDtos
            };

            return new GetPortfolioItemByIdResponse
            {
                Success = true,
                Message = "Item obtenido exitosamente",
                Item = itemDto
            };
        }
        catch (Exception ex)
        {
            return new GetPortfolioItemByIdResponse
            {
                Success = false,
                Message = $"Error al obtener item: {ex.Message}"
            };
        }
    }
}
