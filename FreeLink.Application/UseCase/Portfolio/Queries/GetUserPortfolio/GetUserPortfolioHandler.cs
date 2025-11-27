using FreeLink.Application.UseCase.Portfolio.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Queries.GetUserPortfolio;

public class GetUserPortfolioHandler : IRequestHandler<GetUserPortfolioQuery, GetUserPortfolioResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserPortfolioHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserPortfolioResponse> Handle(GetUserPortfolioQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new GetUserPortfolioResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener items de portafolio
            var portfolioItems = await _unitOfWork.Repository<Portfolioitem>()
                .GetAsync(p => p.UserId == request.UserId);

            var itemsList = portfolioItems.OrderByDescending(p => p.CreatedAt).ToList();

            // Mapear a DTOs con archivos
            var itemDtos = new List<PortfolioItemDto>();
            foreach (var item in itemsList)
            {
                // Obtener archivos del item
                var files = await _unitOfWork.Repository<Portfoliofile>()
                    .GetAsync(f => f.PortfolioId == item.PortfolioId);

                var fileDtos = files.Select(f => new PortfolioFileDto
                {
                    FileId = f.FileId,
                    PortfolioId = f.PortfolioId,
                    FileUrl = f.FileUrl,
                    FileType = f.FileType,
                    FileSize = f.FileSize.HasValue ? f.FileSize.Value : 0,
                    UploadedAt = f.UploadedAt
                }).ToList();

                itemDtos.Add(new PortfolioItemDto
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
                });
            }

            return new GetUserPortfolioResponse
            {
                Success = true,
                Message = "Portafolio obtenido exitosamente",
                Items = itemDtos
            };
        }
        catch (Exception ex)
        {
            return new GetUserPortfolioResponse
            {
                Success = false,
                Message = $"Error al obtener portafolio: {ex.Message}"
            };
        }
    }
}
