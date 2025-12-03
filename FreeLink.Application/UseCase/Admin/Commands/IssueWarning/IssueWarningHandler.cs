using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.IssueWarning;

public class IssueWarningHandler : IRequestHandler<IssueWarningCommand, IssueWarningResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public IssueWarningHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<IssueWarningResponse> Handle(IssueWarningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new IssueWarningResponse
                {
                    Success = false,
                    Message = "No tienes permisos para emitir advertencias"
                };
            }

            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new IssueWarningResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Validar razón
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return new IssueWarningResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una razón para la advertencia"
                };
            }

            // Crear la sanción
            var sanction = new Usersanction
            {
                UserId = request.UserId,
                SanctionType = "Advertencia",
                Reason = request.Reason,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                IsActive = true,
                AppliedBy = request.RequestingAdminId
            };

            await _unitOfWork.Repository<Usersanction>().Add(sanction);

            // Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                request.UserId,
                "admin_action",
                "Advertencia Recibida",
                $"Has recibido una advertencia. Razón: {request.Reason}",
                "user",
                request.UserId
            );

            // Registrar actividad del administrador
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "IssueWarning",
                $"Emitió advertencia al usuario #{request.UserId}"
            );

            await _unitOfWork.Complete();

            return new IssueWarningResponse
            {
                Success = true,
                Message = "Advertencia emitida exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new IssueWarningResponse
            {
                Success = false,
                Message = $"Error al emitir advertencia: {ex.Message}"
            };
        }
    }
}
