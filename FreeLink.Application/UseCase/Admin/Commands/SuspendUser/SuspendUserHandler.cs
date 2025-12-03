using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.SuspendUser;

public class SuspendUserHandler : IRequestHandler<SuspendUserCommand, SuspendUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public SuspendUserHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<SuspendUserResponse> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new SuspendUserResponse
                {
                    Success = false,
                    Message = "No tienes permisos para suspender usuarios"
                };
            }

            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new SuspendUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Validar razón y duración
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return new SuspendUserResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una razón para la suspensión"
                };
            }

            if (request.DurationDays <= 0)
            {
                return new SuspendUserResponse
                {
                    Success = false,
                    Message = "La duración debe ser mayor a 0 días"
                };
            }

            // Crear la sanción
            var endDate = DateTime.UtcNow.AddDays(request.DurationDays);
            var sanction = new Usersanction
            {
                UserId = request.UserId,
                SanctionType = "Suspensión Temporal",
                Reason = request.Reason,
                StartDate = DateTime.UtcNow,
                EndDate = endDate,
                IsActive = true,
                AppliedBy = request.RequestingAdminId
            };

            await _unitOfWork.Repository<Usersanction>().Add(sanction);

            // Desactivar al usuario temporalmente
            user.IsActive = false;
            await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);

            // Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                request.UserId,
                "admin_action",
                "Cuenta Suspendida",
                $"Tu cuenta ha sido suspendida hasta {endDate:dd/MM/yyyy}. Razón: {request.Reason}",
                "user",
                request.UserId
            );

            // Registrar actividad del administrador
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "SuspendUser",
                $"Suspendió al usuario #{request.UserId} por {request.DurationDays} días"
            );

            await _unitOfWork.Complete();

            return new SuspendUserResponse
            {
                Success = true,
                Message = $"Usuario suspendido hasta {endDate:dd/MM/yyyy}"
            };
        }
        catch (Exception ex)
        {
            return new SuspendUserResponse
            {
                Success = false,
                Message = $"Error al suspender usuario: {ex.Message}"
            };
        }
    }
}
