using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.AssignDispute;

public class AssignDisputeHandler : IRequestHandler<AssignDisputeCommand, AssignDisputeResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public AssignDisputeHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<AssignDisputeResponse> Handle(AssignDisputeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var requestingAdmin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (requestingAdmin == null || requestingAdmin.UserType != "Administrador")
            {
                return new AssignDisputeResponse
                {
                    Success = false,
                    Message = "No tienes permisos para asignar disputas"
                };
            }

            // Verificar que el mediador es administrador
            var mediator = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.MediatorId);
            if (mediator == null || mediator.UserType != "Administrador")
            {
                return new AssignDisputeResponse
                {
                    Success = false,
                    Message = "El mediador asignado no existe o no es administrador"
                };
            }

            // Obtener la disputa
            var dispute = await _unitOfWork.Repository<Dispute>().GetById(request.DisputeId);
            if (dispute == null)
            {
                return new AssignDisputeResponse
                {
                    Success = false,
                    Message = "Disputa no encontrada"
                };
            }

            // Asignar mediador
            dispute.MediatorId = request.MediatorId;

            // Si estaba abierta, cambiar a "En Revisión"
            if (dispute.DisputeStatus == "Abierta")
            {
                dispute.DisputeStatus = "En Revisión";
            }

            await _unitOfWork.Repository<Dispute>().Update(dispute);

            // Notificar al iniciador y respondente
            await _notificationService.CreateNotificationAsync(
                dispute.InitiatorId,
                "admin_action",
                "Disputa Asignada a Mediador",
                $"Tu disputa #{dispute.DisputeId} está siendo revisada por un mediador",
                "dispute",
                dispute.DisputeId
            );

            await _notificationService.CreateNotificationAsync(
                dispute.RespondentId,
                "admin_action",
                "Disputa Asignada a Mediador",
                $"La disputa #{dispute.DisputeId} está siendo revisada por un mediador",
                "dispute",
                dispute.DisputeId
            );

            // Notificar al mediador asignado
            await _notificationService.CreateNotificationAsync(
                request.MediatorId,
                "admin_action",
                "Disputa Asignada",
                $"Se te ha asignado la mediación de la disputa #{dispute.DisputeId}",
                "dispute",
                dispute.DisputeId
            );

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "AssignDispute",
                $"Asignó disputa #{request.DisputeId} al mediador #{request.MediatorId}"
            );

            await _unitOfWork.Complete();

            return new AssignDisputeResponse
            {
                Success = true,
                Message = "Disputa asignada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new AssignDisputeResponse
            {
                Success = false,
                Message = $"Error al asignar disputa: {ex.Message}"
            };
        }
    }
}
