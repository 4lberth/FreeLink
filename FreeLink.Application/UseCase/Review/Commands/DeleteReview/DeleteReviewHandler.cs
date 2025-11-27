using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.DeleteReview;

public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, DeleteReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteReviewResponse> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener review
            var review = await _unitOfWork.Repository<Domain.Entities.Review>().GetById(request.ReviewId);
            if (review == null)
            {
                return new DeleteReviewResponse
                {
                    Success = false,
                    Message = "Calificación no encontrada"
                };
            }

            // Validar que solo el creador puede eliminar
            if (review.ReviewerId != request.RequestingUserId)
            {
                return new DeleteReviewResponse
                {
                    Success = false,
                    Message = "No tienes permiso para eliminar esta calificación"
                };
            }

            var reviewedUserId = review.ReviewedUserId;

            // Eliminar review
            await _unitOfWork.Repository<Domain.Entities.Review>().Delete(review);
            await _unitOfWork.Complete();

            // Recalcular promedio sin esta review
            var allReviews = await _unitOfWork.Repository<Domain.Entities.Review>()
                .GetAsync(r => r.ReviewedUserId == reviewedUserId);

            var averageRating = allReviews.Any() 
                ? Math.Round((double)allReviews.Average(r => r.Rating), 2) 
                : 0;

            // Actualizar el promedio en Freelancerprofile
            var freelancerProfile = (await _unitOfWork.Repository<Freelancerprofile>()
                .GetAsync(f => f.UserId == reviewedUserId))
                .FirstOrDefault();

            if (freelancerProfile != null)
            {
                freelancerProfile.AverageRating = (decimal)averageRating;
                await _unitOfWork.Complete();
            }

            return new DeleteReviewResponse
            {
                Success = true,
                Message = "Calificación eliminada exitosamente",
                NewAverageRating = averageRating
            };
        }
        catch (Exception ex)
        {
            return new DeleteReviewResponse
            {
                Success = false,
                Message = $"Error al eliminar calificación: {ex.Message}"
            };
        }
    }
}
