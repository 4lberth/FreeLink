using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, CreateReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateReviewResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar rating (debe estar entre 1.0 y 5.0)
            if (request.Rating < 1.0m || request.Rating > 5.0m)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "La calificación debe estar entre 1.0 y 5.0"
                };
            }

            // 2. Verificar que el proyecto existe
            var project = await _unitOfWork.Repository<Domain.Entities.Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Verificar que no exista ya una calificación del mismo revisor para el mismo proyecto y usuario
            var existingReview = await _unitOfWork.Repository<Review>()
                .GetFirstOrDefaultAsync(r => r.ProjectId == request.ProjectId 
                    && r.ReviewerId == request.ReviewerId 
                    && r.ReviewedUserId == request.ReviewedUserId);

            if (existingReview != null)
            {
                return new CreateReviewResponse
                {
                    Success = false,
                    Message = "Ya existe una calificación para este proyecto y usuario"
                };
            }

            // 4. Crear la calificación
            var review = new Review
            {
                ProjectId = request.ProjectId,
                ReviewerId = request.ReviewerId,
                ReviewedUserId = request.ReviewedUserId,
                Rating = request.Rating,
                ReviewText = request.ReviewText,
                ReviewType = request.ReviewType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Review>().Add(review);
            await _unitOfWork.Complete();

            // 5. Calcular y actualizar el promedio de calificaciones del usuario calificado
            var allReviews = await _unitOfWork.Repository<Review>()
                .GetAsync(r => r.ReviewedUserId == request.ReviewedUserId);

            var reviewsList = allReviews.ToList();
            var averageRating = reviewsList.Any() 
                ? reviewsList.Average(r => (double)r.Rating) 
                : 0.0;

            // 6. Si el usuario calificado es un Freelancer, actualizar su perfil
            var reviewedUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.ReviewedUserId);
            if (reviewedUser != null && reviewedUser.UserType == "Freelancer")
            {
                var freelancerProfile = await _unitOfWork.Repository<Freelancerprofile>()
                    .GetFirstOrDefaultAsync(fp => fp.UserId == request.ReviewedUserId);

                if (freelancerProfile != null)
                {
                    freelancerProfile.AverageRating = (decimal)averageRating;
                    freelancerProfile.TotalReviews = reviewsList.Count;
                    await _unitOfWork.Repository<Freelancerprofile>().Update(freelancerProfile);
                }
            }

            // 7. Guardar cambios
            await _unitOfWork.Complete();

            return new CreateReviewResponse
            {
                Success = true,
                Message = "Calificación creada exitosamente",
                ReviewId = review.ReviewId,
                NewAverageRating = (decimal)averageRating
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

