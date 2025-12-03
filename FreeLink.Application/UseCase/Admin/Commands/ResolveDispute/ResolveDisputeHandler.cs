using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ResolveDispute;

public class ResolveDisputeHandler : IRequestHandler<ResolveDisputeCommand, ResolveDisputeResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public ResolveDisputeHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<ResolveDisputeResponse> Handle(ResolveDisputeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new ResolveDisputeResponse
                {
                    Success = false,
                    Message = "No tienes permisos para resolver disputas"
                };
            }

            // Obtener la disputa
            var dispute = await _unitOfWork.Repository<Dispute>().GetById(request.DisputeId);
            if (dispute == null)
            {
                return new ResolveDisputeResponse
                {
                    Success = false,
                    Message = "Disputa no encontrada"
                };
            }

            // Validar resolución
            if (string.IsNullOrWhiteSpace(request.Resolution))
            {
                return new ResolveDisputeResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una resolución"
                };
            }

            // Validar que winning party es válido si se proporcionó
            if (request.WinningPartyId.HasValue)
            {
                if (request.WinningPartyId.Value != dispute.InitiatorId &&
                    request.WinningPartyId.Value != dispute.RespondentId)
                {
                    return new ResolveDisputeResponse
                    {
                        Success = false,
                        Message = "La parte ganadora debe ser el iniciador o el respondente de la disputa"
                    };
                }
            }

            // Resolver la disputa
            dispute.DisputeStatus = "Resuelta";
            dispute.Resolution = request.Resolution;
            dispute.ResolvedAt = DateTime.UtcNow;

            if (!dispute.MediatorId.HasValue)
            {
                dispute.MediatorId = request.RequestingAdminId;
            }

            await _unitOfWork.Repository<Dispute>().Update(dispute);

            // Notificar a ambas partes
            var winningMessage = request.WinningPartyId.HasValue
                ? (request.WinningPartyId == dispute.InitiatorId ? " A tu favor." : " En tu contra.")
                : "";

            await _notificationService.CreateNotificationAsync(
                dispute.InitiatorId,
                "admin_action",
                "Disputa Resuelta",
                $"La disputa #{dispute.DisputeId} ha sido resuelta.{winningMessage} Resolución: {request.Resolution}",
                "dispute",
                dispute.DisputeId
            );

            await _notificationService.CreateNotificationAsync(
                dispute.RespondentId,
                "admin_action",
                "Disputa Resuelta",
                $"La disputa #{dispute.DisputeId} ha sido resuelta.{(request.WinningPartyId.HasValue ? (request.WinningPartyId == dispute.RespondentId ? " A tu favor." : " En tu contra.") : "")} Resolución: {request.Resolution}",
                "dispute",
                dispute.DisputeId
            );

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "ResolveDispute",
                $"Resolvió disputa #{request.DisputeId}"
            );

            await _unitOfWork.Complete();

            return new ResolveDisputeResponse
            {
                Success = true,
                Message = "Disputa resuelta exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ResolveDisputeResponse
            {
                Success = false,
                Message = $"Error al resolver disputa: {ex.Message}"
            };
        }
    }
}
