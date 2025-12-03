using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ApproveVerification;

public class ApproveVerificationHandler : IRequestHandler<ApproveVerificationCommand, ApproveVerificationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public ApproveVerificationHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<ApproveVerificationResponse> Handle(ApproveVerificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new ApproveVerificationResponse
                {
                    Success = false,
                    Message = "No tienes permisos para aprobar verificaciones"
                };
            }

            // Obtener la verificación
            var verification = await _unitOfWork.Repository<Identityverification>().GetById(request.VerificationId);
            if (verification == null)
            {
                return new ApproveVerificationResponse
                {
                    Success = false,
                    Message = "Verificación no encontrada"
                };
            }

            // Validar que esté pendiente
            if (verification.VerificationStatus != "Pendiente")
            {
                return new ApproveVerificationResponse
                {
                    Success = false,
                    Message = $"La verificación ya fue {verification.VerificationStatus?.ToLower()}"
                };
            }

            // Actualizar verificación
            verification.VerificationStatus = "Aprobada";
            verification.ReviewedAt = DateTime.UtcNow;
            verification.ReviewedBy = request.RequestingAdminId;

            await _unitOfWork.Repository<Identityverification>().Update(verification);

            // Actualizar usuario como verificado
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(verification.UserId);
            if (user != null)
            {
                user.IsVerified = true;
                await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);
            }

            await _unitOfWork.Complete();

            // Crear notificación al usuario
            await _notificationService.CreateNotificationAsync(
                userId: verification.UserId,
                type: "Verification",
                title: "Verificación Aprobada",
                message: "Tu verificación de identidad ha sido aprobada exitosamente. Ahora tienes acceso a todas las funcionalidades de la plataforma.",
                resourceType: "Verification",
                resourceId: verification.VerificationId
            );

            // Registrar actividad del admin
            var adminLog = new Adminactivitylog
            {
                AdminId = request.RequestingAdminId,
                ActionType = "VerificationApproved",
                ActionDescription = $"Aprobó verificación de identidad",
                TargetUserId = verification.UserId,
                TargetResourceType = "Verification",
                TargetResourceId = verification.VerificationId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Adminactivitylog>().Add(adminLog);
            await _unitOfWork.Complete();

            return new ApproveVerificationResponse
            {
                Success = true,
                Message = "Verificación aprobada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApproveVerificationResponse
            {
                Success = false,
                Message = $"Error al aprobar verificación: {ex.Message}"
            };
        }
    }
}
