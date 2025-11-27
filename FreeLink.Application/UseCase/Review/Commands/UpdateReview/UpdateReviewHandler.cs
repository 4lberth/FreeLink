using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.UpdateReview;

public class UpdateReviewHandler : IRequestHandler<UpdateReviewCommand, UpdateReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateReviewResponse> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener review
            var review = await _unitOfWork.Repository<Domain.Entities.Review>().GetById(request.ReviewId);
            if (review == null)
            {
                return new UpdateReviewResponse
                {
                    Success = false,
                    Message = "Calificación no encontrada"
                };
            }

            // Validar que solo el creador puede actualizar
            if (review.ReviewerId != request.RequestingUserId)
            {
                return new UpdateReviewResponse
                {
                    Success = false,
                    Message = "No tienes permiso para actualizar esta calificación"
                };
            }

            // Validar rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return new UpdateReviewResponse
                {
                    Success = false,
                    Message = "La calificación debe estar entre 1 y 5"
                };
            }

            // Actualizar review
            review.Rating = request.Rating;
            review.ReviewText = request.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complete();

            // Recalcular promedio
            var allReviews = await _unitOfWork.Repository<Domain.Entities.Review>()
                .GetAsync(r => r.ReviewedUserId == review.ReviewedUserId);

            var averageRating = allReviews.Any() 
                ? Math.Round((double)allReviews.Average(r => r.Rating), 2) 
                : 0;

            // Actualizar el promedio en Freelancerprofile
            var freelancerProfile = (await _unitOfWork.Repository<Freelancerprofile>()
                .GetAsync(f => f.UserId == review.ReviewedUserId))
                .FirstOrDefault();

            if (freelancerProfile != null)
            {
                freelancerProfile.AverageRating = (decimal)averageRating;
                await _unitOfWork.Complete();
            }

            return new UpdateReviewResponse
            {
                Success = true,
                Message = "Calificación actualizada exitosamente",
                NewAverageRating = averageRating
            };
        }
        catch (Exception ex)
        {
            return new UpdateReviewResponse
            {
                Success = false,
                Message = $"Error al actualizar calificación: {ex.Message}"
            };
        }
    }
}
