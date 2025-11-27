using FreeLink.Application.UseCase.Review.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Review.Queries.GetUserReviews;

public class GetUserReviewsHandler : IRequestHandler<GetUserReviewsQuery, GetUserReviewsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserReviewsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserReviewsResponse> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new GetUserReviewsResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener todas las reviews del usuario
            var allReviews = await _unitOfWork.Repository<Domain.Entities.Review>()
                .GetAsync(r => r.ReviewedUserId == request.UserId);

            var reviewsList = allReviews.OrderByDescending(r => r.CreatedAt).ToList();

            // Calcular paginación
            var totalCount = reviewsList.Count;
            var reviews = reviewsList
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Mapear a DTOs
            var reviewDtos = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                var reviewer = await _unitOfWork.Repository<Domain.Entities.User>().GetById(review.ReviewerId);
                
                // Obtener respuesta si existe
                var reviewResponse = (await _unitOfWork.Repository<Reviewresponse>()
                    .GetAsync(rr => rr.ReviewId == review.ReviewId))
                    .FirstOrDefault();
                
                reviewDtos.Add(new ReviewDto
                {
                    ReviewId = review.ReviewId,
                    ProjectId = review.ProjectId,
                    ReviewerId = review.ReviewerId,
                    ReviewerName = reviewer?.Email,
                    ReviewedUserId = review.ReviewedUserId,
                    ReviewedUserName = user.Email,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    CreatedAt = review.CreatedAt,
                    UpdatedAt = review.UpdatedAt,
                    Response = reviewResponse != null ? new ReviewResponseDto
                    {
                        ResponseId = reviewResponse.ResponseId,
                        ReviewId = reviewResponse.ReviewId,
                        UserId = review.ReviewedUserId,
                        ResponseText = reviewResponse.ResponseText,
                        CreatedAt = reviewResponse.CreatedAt
                    } : null
                });
            }

            // Calcular promedio
            var averageRating = reviewsList.Any() 
                ? Math.Round((double)reviewsList.Average(r => r.Rating), 2) 
                : 0;

            return new GetUserReviewsResponse
            {
                Success = true,
                Message = "Reviews obtenidas exitosamente",
                Reviews = reviewDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                AverageRating = averageRating
            };
        }
        catch (Exception ex)
        {
            return new GetUserReviewsResponse
            {
                Success = false,
                Message = $"Error al obtener reviews: {ex.Message}"
            };
        }
    }
}
