using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.RejectVerification;

public class RejectVerificationHandler : IRequestHandler<RejectVerificationCommand, RejectVerificationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RejectVerificationHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<RejectVerificationResponse> Handle(RejectVerificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar razón de rechazo
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                return new RejectVerificationResponse
                {
                    Success = false,
                    Message = "Debes proporcionar una razón para rechazar la verificación"
                };
            }

            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new RejectVerificationResponse
                {
                    Success = false,
                    Message = "No tienes permisos para rechazar verificaciones"
                };
            }

            // Obtener la verificación
            var verification = await _unitOfWork.Repository<Identityverification>().GetById(request.VerificationId);
            if (verification == null)
            {
                return new RejectVerificationResponse
                {
                    Success = false,
                    Message = "Verificación no encontrada"
                };
            }

            // Validar que esté pendiente
            if (verification.VerificationStatus != "Pendiente")
            {
                return new RejectVerificationResponse
                {
                    Success = false,
                    Message = $"La verificación ya fue {verification.VerificationStatus?.ToLower()}"
                };
            }

            // Actualizar verificación
            verification.VerificationStatus = "Rechazado";
            verification.ReviewedAt = DateTime.UtcNow;
            verification.ReviewedBy = request.RequestingAdminId;
            verification.RejectionReason = request.RejectionReason;

            await _unitOfWork.Repository<Identityverification>().Update(verification);
            await _unitOfWork.Complete();

            // Crear notificación al usuario
            await _notificationService.CreateNotificationAsync(
                userId: verification.UserId,
                type: "Verification",
                title: "Verificación Rechazada",
                message: $"Tu verificación de identidad ha sido rechazada. Razón: {request.RejectionReason}. Por favor, envía documentos válidos.",
                resourceType: "Verification",
                resourceId: verification.VerificationId
            );

            // Registrar actividad del admin
            var adminLog = new Adminactivitylog
            {
                AdminId = request.RequestingAdminId,
                ActionType = "VerificationRejected",
                ActionDescription = $"Rechazó verificación de identidad. Razón: {request.RejectionReason}",
                TargetUserId = verification.UserId,
                TargetResourceType = "Verification",
                TargetResourceId = verification.VerificationId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Adminactivitylog>().Add(adminLog);
            await _unitOfWork.Complete();

            return new RejectVerificationResponse
            {
                Success = true,
                Message = "Verificación rechazada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new RejectVerificationResponse
            {
                Success = false,
                Message = $"Error al rechazar verificación: {ex.Message}"
            };
        }
    }
}
