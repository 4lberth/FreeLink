using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.RejectWithdrawal;

public class RejectWithdrawalHandler : IRequestHandler<RejectWithdrawalCommand, RejectWithdrawalResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public RejectWithdrawalHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<RejectWithdrawalResponse> Handle(RejectWithdrawalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.AdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new RejectWithdrawalResponse
                {
                    Success = false,
                    Message = "No tienes permisos para rechazar retiros"
                };
            }

            // 2. Validar razón de rechazo
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                return new RejectWithdrawalResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una razón para rechazar el retiro"
                };
            }

            // 3. Obtener la transacción de retiro
            var withdrawal = await _unitOfWork.Repository<Transaction>().GetById(request.WithdrawalId);
            if (withdrawal == null)
            {
                return new RejectWithdrawalResponse
                {
                    Success = false,
                    Message = "Retiro no encontrado"
                };
            }

            // 4. Verificar que sea un retiro y esté pendiente
            if (withdrawal.TransactionType != "Retiro")
            {
                return new RejectWithdrawalResponse
                {
                    Success = false,
                    Message = "La transacción no es un retiro"
                };
            }

            if (withdrawal.TransactionStatus != "Pendiente")
            {
                return new RejectWithdrawalResponse
                {
                    Success = false,
                    Message = $"El retiro ya fue procesado. Estado actual: {withdrawal.TransactionStatus}"
                };
            }

            // 5. Rechazar el retiro
            withdrawal.TransactionStatus = "Rechazada";
            withdrawal.Description += $" | Rechazado: {request.RejectionReason}";

            await _unitOfWork.Repository<Transaction>().Update(withdrawal);
            await _unitOfWork.Complete();

            // 6. Notificar al usuario
            if (withdrawal.FromUserId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    userId: withdrawal.FromUserId.Value,
                    type: "WithdrawalRejected",
                    title: "Retiro Rechazado",
                    message: $"Tu solicitud de retiro de ${withdrawal.Amount:F2} fue rechazada. Razón: {request.RejectionReason}",
                    resourceType: "Transaction",
                    resourceId: withdrawal.TransactionId
                );
            }

            // 7. Registrar actividad del admin
            await _activityLogger.LogActivity(
                adminId: request.AdminId,
                actionType: "RejectWithdrawal",
                actionDetails: $"Rechazó retiro ID {request.WithdrawalId} por ${withdrawal.Amount:F2} para usuario {withdrawal.FromUserId}. Razón: {request.RejectionReason}"
            );

            return new RejectWithdrawalResponse
            {
                Success = true,
                Message = "Retiro rechazado exitosamente",
                WithdrawalId = withdrawal.TransactionId,
                Status = "Rechazada"
            };
        }
        catch (Exception ex)
        {
            return new RejectWithdrawalResponse
            {
                Success = false,
                Message = $"Error al rechazar retiro: {ex.Message}"
            };
        }
    }
}
