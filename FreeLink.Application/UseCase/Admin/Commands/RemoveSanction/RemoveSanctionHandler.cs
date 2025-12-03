using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.RemoveSanction;

public class RemoveSanctionHandler : IRequestHandler<RemoveSanctionCommand, RemoveSanctionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public RemoveSanctionHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<RemoveSanctionResponse> Handle(RemoveSanctionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new RemoveSanctionResponse
                {
                    Success = false,
                    Message = "No tienes permisos para remover sanciones"
                };
            }

            // Obtener la sanción
            var sanction = await _unitOfWork.Repository<Usersanction>().GetById(request.SanctionId);
            if (sanction == null)
            {
                return new RemoveSanctionResponse
                {
                    Success = false,
                    Message = "Sanción no encontrada"
                };
            }

            // Validar razón
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return new RemoveSanctionResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una razón para remover la sanción"
                };
            }

            // Desactivar la sanción
            sanction.IsActive = false;
            sanction.EndDate = DateTime.UtcNow;
            await _unitOfWork.Repository<Usersanction>().Update(sanction);

            // Si era una suspensión o baneo, reactivar al usuario si no tiene otras sanciones activas
            if (sanction.SanctionType == "Suspensión Temporal" || sanction.SanctionType == "Baneo Permanente")
            {
                var allSanctions = await _unitOfWork.Repository<Usersanction>().GetAll();
                var activeBlockingSanctions = allSanctions
                    .Where(s => s.UserId == sanction.UserId &&
                               s.IsActive == true &&
                               s.SanctionId != sanction.SanctionId &&
                               (s.SanctionType == "Suspensión Temporal" || s.SanctionType == "Baneo Permanente"))
                    .Any();

                if (!activeBlockingSanctions)
                {
                    var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(sanction.UserId);
                    if (user != null)
                    {
                        user.IsActive = true;
                        await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);
                    }
                }
            }

            // Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                sanction.UserId,
                "admin_action",
                "Sanción Removida",
                $"Una sanción en tu cuenta ha sido removida. Razón: {request.Reason}",
                "sanction",
                sanction.SanctionId
            );

            // Registrar actividad del administrador
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "RemoveSanction",
                $"Removió sanción #{request.SanctionId} del usuario #{sanction.UserId}"
            );

            await _unitOfWork.Complete();

            return new RemoveSanctionResponse
            {
                Success = true,
                Message = "Sanción removida exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new RemoveSanctionResponse
            {
                Success = false,
                Message = $"Error al remover sanción: {ex.Message}"
            };
        }
    }
}
