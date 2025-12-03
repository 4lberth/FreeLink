using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.UpdateDisputeStatus;

public class UpdateDisputeStatusHandler : IRequestHandler<UpdateDisputeStatusCommand, UpdateDisputeStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public UpdateDisputeStatusHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<UpdateDisputeStatusResponse> Handle(UpdateDisputeStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new UpdateDisputeStatusResponse
                {
                    Success = false,
                    Message = "No tienes permisos para actualizar disputas"
                };
            }

            // Obtener la disputa
            var dispute = await _unitOfWork.Repository<Dispute>().GetById(request.DisputeId);
            if (dispute == null)
            {
                return new UpdateDisputeStatusResponse
                {
                    Success = false,
                    Message = "Disputa no encontrada"
                };
            }

            // Validar estado
            var validStatuses = new[] { "Abierta", "En Revisión", "Resuelta", "Cerrada" };
            if (!validStatuses.Contains(request.NewStatus))
            {
                return new UpdateDisputeStatusResponse
                {
                    Success = false,
                    Message = "Estado inválido. Debe ser: Abierta, En Revisión, Resuelta o Cerrada"
                };
            }

            var oldStatus = dispute.DisputeStatus;
            dispute.DisputeStatus = request.NewStatus;

            await _unitOfWork.Repository<Dispute>().Update(dispute);

            // Notificar a ambas partes
            await _notificationService.CreateNotificationAsync(
                dispute.InitiatorId,
                "admin_action",
                "Estado de Disputa Actualizado",
                $"La disputa #{dispute.DisputeId} cambió de '{oldStatus}' a '{request.NewStatus}'",
                "dispute",
                dispute.DisputeId
            );

            await _notificationService.CreateNotificationAsync(
                dispute.RespondentId,
                "admin_action",
                "Estado de Disputa Actualizado",
                $"La disputa #{dispute.DisputeId} cambió de '{oldStatus}' a '{request.NewStatus}'",
                "dispute",
                dispute.DisputeId
            );

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "UpdateDisputeStatus",
                $"Actualizó disputa #{request.DisputeId} a estado '{request.NewStatus}'"
            );

            await _unitOfWork.Complete();

            return new UpdateDisputeStatusResponse
            {
                Success = true,
                Message = "Estado de disputa actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateDisputeStatusResponse
            {
                Success = false,
                Message = $"Error al actualizar disputa: {ex.Message}"
            };
        }
    }
}
