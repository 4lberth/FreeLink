using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.RespondToReview;

public class RespondToReviewHandler : IRequestHandler<RespondToReviewCommand, RespondToReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RespondToReviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RespondToReviewResponse> Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener review
            var review = await _unitOfWork.Repository<Domain.Entities.Review>().GetById(request.ReviewId);
            if (review == null)
            {
                return new RespondToReviewResponse
                {
                    Success = false,
                    Message = "Calificación no encontrada"
                };
            }

            // Solo el usuario calificado puede responder
            if (review.ReviewedUserId != request.RequestingUserId)
            {
                return new RespondToReviewResponse
                {
                    Success = false,
                    Message = "Solo el usuario calificado puede responder a esta review"
                };
            }

            // Validar que la respuesta no esté vacía
            if (string.IsNullOrWhiteSpace(request.ResponseText))
            {
                return new RespondToReviewResponse
                {
                    Success = false,
                    Message = "La respuesta no puede estar vacía"
                };
            }

            // Crear o actualizar la respuesta en la tabla Reviewresponse
            var existingResponse = (await _unitOfWork.Repository<Reviewresponse>()
                .GetAsync(r => r.ReviewId == request.ReviewId))
                .FirstOrDefault();

            if (existingResponse != null)
            {
                // Actualizar respuesta existente
                existingResponse.ResponseText = request.ResponseText;
                existingResponse.CreatedAt = DateTime.UtcNow;
                await _unitOfWork.Complete();

                return new RespondToReviewResponse
                {
                    Success = true,
                    Message = "Respuesta actualizada exitosamente",
                    ResponseId = existingResponse.ResponseId
                };
            }
            else
            {
                // Crear nueva respuesta
                var newResponse = new Reviewresponse
                {
                    ReviewId = request.ReviewId,
                    ResponseText = request.ResponseText,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Reviewresponse>().Add(newResponse);
                await _unitOfWork.Complete();

                return new RespondToReviewResponse
                {
                    Success = true,
                    Message = "Respuesta agregada exitosamente",
                    ResponseId = newResponse.ResponseId
                };
            }
        }
        catch (Exception ex)
        {
            return new RespondToReviewResponse
            {
                Success = false,
                Message = $"Error al responder: {ex.Message}"
            };
        }
    }
}
