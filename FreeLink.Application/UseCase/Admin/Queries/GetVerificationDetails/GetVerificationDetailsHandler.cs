using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetVerificationDetails;

public class GetVerificationDetailsHandler : IRequestHandler<GetVerificationDetailsQuery, GetVerificationDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVerificationDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetVerificationDetailsResponse> Handle(GetVerificationDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetVerificationDetailsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver verificaciones"
                };
            }

            // Obtener la verificación
            var verification = await _unitOfWork.Repository<Identityverification>().GetById(request.VerificationId);
            if (verification == null)
            {
                return new GetVerificationDetailsResponse
                {
                    Success = false,
                    Message = "Verificación no encontrada"
                };
            }

            // Obtener usuario
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(verification.UserId);
            if (user == null)
            {
                return new GetVerificationDetailsResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener revisor si existe
            string? reviewerName = null;
            if (verification.ReviewedBy.HasValue)
            {
                var reviewer = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(verification.ReviewedBy.Value);
                reviewerName = reviewer?.Email;
            }

            // Mapear a DTO
            var dto = new VerificationDetailsDto
            {
                VerificationId = verification.VerificationId,
                UserId = user.UserId,
                UserName = user.Email ?? "Usuario",
                UserEmail = user.Email ?? "",
                UserType = user.UserType,
                DocumentType = verification.DocumentType,
                DocumentNumber = verification.DocumentNumber,
                DocumentFrontUrl = verification.DocumentFrontUrl,
                DocumentBackUrl = verification.DocumentBackUrl,
                SelfieUrl = verification.SelfieUrl,
                VerificationStatus = verification.VerificationStatus,
                SubmittedAt = verification.SubmittedAt,
                ReviewedAt = verification.ReviewedAt,
                ReviewedBy = verification.ReviewedBy,
                ReviewedByName = reviewerName,
                RejectionReason = verification.RejectionReason
            };

            return new GetVerificationDetailsResponse
            {
                Success = true,
                Message = "Detalles de verificación obtenidos exitosamente",
                Data = dto
            };
        }
        catch (Exception ex)
        {
            return new GetVerificationDetailsResponse
            {
                Success = false,
                Message = $"Error al obtener detalles de verificación: {ex.Message}"
            };
        }
    }
}
