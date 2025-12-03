using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ApproveWithdrawal;

public class ApproveWithdrawalHandler : IRequestHandler<ApproveWithdrawalCommand, ApproveWithdrawalResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public ApproveWithdrawalHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<ApproveWithdrawalResponse> Handle(ApproveWithdrawalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.AdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new ApproveWithdrawalResponse
                {
                    Success = false,
                    Message = "No tienes permisos para aprobar retiros"
                };
            }

            // 2. Obtener la transacción de retiro
            var withdrawal = await _unitOfWork.Repository<Transaction>().GetById(request.WithdrawalId);
            if (withdrawal == null)
            {
                return new ApproveWithdrawalResponse
                {
                    Success = false,
                    Message = "Retiro no encontrado"
                };
            }

            // 3. Verificar que sea un retiro y esté pendiente
            if (withdrawal.TransactionType != "Retiro")
            {
                return new ApproveWithdrawalResponse
                {
                    Success = false,
                    Message = "La transacción no es un retiro"
                };
            }

            if (withdrawal.TransactionStatus != "Pendiente")
            {
                return new ApproveWithdrawalResponse
                {
                    Success = false,
                    Message = $"El retiro ya fue procesado. Estado actual: {withdrawal.TransactionStatus}"
                };
            }

            // 4. Aprobar el retiro
            withdrawal.TransactionStatus = "Completada";
            withdrawal.CompletedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.AdminNotes))
            {
                withdrawal.Description += $" | Notas admin: {request.AdminNotes}";
            }

            await _unitOfWork.Repository<Transaction>().Update(withdrawal);
            await _unitOfWork.Complete();

            // 5. Notificar al usuario
            if (withdrawal.FromUserId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    userId: withdrawal.FromUserId.Value,
                    type: "WithdrawalApproved",
                    title: "Retiro Aprobado",
                    message: $"Tu retiro de ${withdrawal.Amount:F2} ha sido aprobado y procesado",
                    resourceType: "Transaction",
                    resourceId: withdrawal.TransactionId
                );
            }

            // 6. Registrar actividad del admin
            await _activityLogger.LogActivity(
                adminId: request.AdminId,
                actionType: "ApproveWithdrawal",
                actionDetails: $"Aprobó retiro ID {request.WithdrawalId} por ${withdrawal.Amount:F2} para usuario {withdrawal.FromUserId}"
            );

            return new ApproveWithdrawalResponse
            {
                Success = true,
                Message = "Retiro aprobado exitosamente",
                WithdrawalId = withdrawal.TransactionId,
                Amount = withdrawal.Amount,
                Status = "Completada"
            };
        }
        catch (Exception ex)
        {
            return new ApproveWithdrawalResponse
            {
                Success = false,
                Message = $"Error al aprobar retiro: {ex.Message}"
            };
        }
    }
}
