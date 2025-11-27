using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.CreateReview;

public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, CreateReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateReviewResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el revisor existe
            var reviewer = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.ReviewerId);
            if (reviewer == null)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "Revisor no encontrado"
                };
            }

            // Validar que el usuario revisado existe
            var reviewedUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.ReviewedUserId);
            if (reviewedUser == null)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "Usuario a calificar no encontrado"
                };
            }

            // Validar que no se autocalifique
            if (request.ReviewerId == request.ReviewedUserId)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "No puedes calificarte a ti mismo"
                };
            }

            // Validar rating (1-5)
            if (request.Rating < 1 || request.Rating > 5)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "La calificación debe estar entre 1 y 5"
                };
            }

            // Validar que solo el dueño puede crear su review
            if (request.ReviewerId != request.RequestingUserId)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "No puedes crear reviews en nombre de otros usuarios"
                };
            }

            // Validar proyecto si se proporciona
            if (request.ProjectId.HasValue)
            {
                var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId.Value);
                if (project == null)
                {
                    return new CreateReviewResponse
                    {
                        Success = false,
                        Message = "Proyecto no encontrado"
                    };
                }
            }

            // Crear review
            var review = new Domain.Entities.Review
            {
                ReviewerId = request.ReviewerId,
                ReviewedUserId = request.ReviewedUserId,
                ProjectId = request.ProjectId ?? 0,
                Rating = request.Rating,
                ReviewText = request.Comment,
                ReviewType = "WorkReview",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Domain.Entities.Review>().Add(review);
            await _unitOfWork.Complete();

            // Calcular nuevo promedio del usuario calificado
            var allReviews = await _unitOfWork.Repository<Domain.Entities.Review>()
                .GetAsync(r => r.ReviewedUserId == request.ReviewedUserId);

            var averageRating = allReviews.Any() 
                ? Math.Round((double)allReviews.Average(r => r.Rating), 2) 
                : 0;

            // Actualizar el promedio en Freelancerprofile si es freelancer
            var freelancerProfile = (await _unitOfWork.Repository<Freelancerprofile>()
                .GetAsync(f => f.UserId == request.ReviewedUserId))
                .FirstOrDefault();

            if (freelancerProfile != null)
            {
                freelancerProfile.AverageRating = (decimal)averageRating;
                await _unitOfWork.Complete();
            }

            return new CreateReviewResponse
            {
                Success = true,
                Message = "Calificación creada exitosamente",
                ReviewId = review.ReviewId,
                NewAverageRating = averageRating
            };
        }
        catch (Exception ex)
        {
            return new CreateReviewResponse
            {
                Success = false,
                Message = $"Error al crear calificación: {ex.Message}"
            };
        }
    }
}
