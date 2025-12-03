using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.ReleasePayment;

public class ReleasePaymentHandler : IRequestHandler<ReleasePaymentCommand, ReleasePaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IWalletService _walletService;
    private const decimal CommissionRate = 0.10m; // 10% commission

    public ReleasePaymentHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IWalletService walletService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _walletService = walletService;
    }

    public async Task<ReleasePaymentResponse> Handle(ReleasePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener entregable
            var deliverable = await _unitOfWork.Repository<Projectdeliverable>().GetById(request.DeliverableId);
            if (deliverable == null)
            {
                return new ReleasePaymentResponse
                {
                    Success = false,
                    Message = "Entregable no encontrado"
                };
            }

            // 2. Verificar que el entregable está aprobado
            if (deliverable.DeliverableStatus != "Aprobado")
            {
                return new ReleasePaymentResponse
                {
                    Success = false,
                    Message = "Solo se puede liberar pago de entregables aprobados"
                };
            }

            // 3. Obtener proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(deliverable.ProjectId);
            if (project == null)
            {
                return new ReleasePaymentResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 4. Verificar que el usuario es el cliente
            if (project.ClientId != request.ClientId)
            {
                return new ReleasePaymentResponse
                {
                    Success = false,
                    Message = "Solo el cliente puede liberar pagos"
                };
            }

            // 5. Verificar que existe escrow para este proyecto
            var escrowQuery = await _unitOfWork.Repository<Escrowaccount>()
                .GetAsync(e => e.ProjectId == deliverable.ProjectId);
            var escrow = escrowQuery.FirstOrDefault();

            if (escrow == null)
            {
                return new ReleasePaymentResponse
                {
                    Success = false,
                    Message = "No existe cuenta de garantía para este proyecto"
                };
            }

            // 6. Verificar que el escrow no ha sido ya liberado
            if (escrow.EscrowStatus == "Released")
            {
                return new ReleasePaymentResponse
                {
                    Success = false,
                    Message = "Los fondos ya han sido liberados"
                };
            }

            // 7. Calcular montos
            decimal totalAmount = escrow.TotalAmount;
            decimal commissionAmount = totalAmount * CommissionRate;
            decimal freelancerAmount = totalAmount - commissionAmount;

            // 8. Crear transacción de liberación al freelancer
            var releaseTransaction = new Transaction
            {
                EscrowId = escrow.EscrowId,
                FromUserId = request.ClientId,
                ToUserId = project.AssignedFreelancerId,
                Amount = freelancerAmount,
                TransactionType = "Liberación",
                TransactionStatus = "Completada",
                Description = $"Pago liberado por proyecto: {project.Title}",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Transaction>().Add(releaseTransaction);
            await _unitOfWork.Complete(); // Guardar para obtener TransactionId

            // 9. Actualizar wallet del freelancer
            await _walletService.UpdateWalletForTransactionAsync(
                userId: project.AssignedFreelancerId!.Value,
                transactionType: "Liberación",
                amount: freelancerAmount,
                transactionStatus: "Completada",
                isFromUser: false // El freelancer es ToUserId
            );

            // 10. Crear transacción de comisión
            var commissionTransaction = new Transaction
            {
                EscrowId = escrow.EscrowId,
                FromUserId = request.ClientId,
                ToUserId = null, // La comisión va a la plataforma
                Amount = commissionAmount,
                TransactionType = "Comisión",
                TransactionStatus = "Completada",
                Description = $"Comisión de plataforma ({CommissionRate * 100}%) - Proyecto: {project.Title}",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Transaction>().Add(commissionTransaction);
            await _unitOfWork.Complete();

            // 11. Registrar comisión en tabla de comisiones
            var platformCommission = new Platformcommission
            {
                TransactionId = commissionTransaction.TransactionId,
                ProjectId = project.ProjectId,
                Amount = commissionAmount,
                CommissionRate = CommissionRate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Platformcommission>().Add(platformCommission);

            // 12. Actualizar estado del escrow
            escrow.EscrowStatus = "Released";
            escrow.ReleasedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Escrowaccount>().Update(escrow);

            // 13. Registrar actividad
            var activityLog = new Projectactivitylog
            {
                ProjectId = project.ProjectId,
                UserId = request.ClientId,
                ActivityType = "PaymentReleased",
                ActivityDescription = $"Pago liberado: ${freelancerAmount:F2} (Comisión: ${commissionAmount:F2})",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Projectactivitylog>().Add(activityLog);

            // 14. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: project.AssignedFreelancerId!.Value,
                type: "PaymentReleased",
                title: "Pago liberado",
                message: $"Has recibido un pago de ${freelancerAmount:F2} por el proyecto '{project.Title}'",
                resourceType: "Transaction",
                resourceId: releaseTransaction.TransactionId
            );

            // 15. Notificar al cliente sobre la comisión
            await _notificationService.CreateNotificationAsync(
                userId: request.ClientId,
                type: "CommissionCharged",
                title: "Comisión aplicada",
                message: $"Se ha aplicado una comisión de ${commissionAmount:F2} ({CommissionRate * 100}%)",
                resourceType: "Transaction",
                resourceId: commissionTransaction.TransactionId
            );

            await _unitOfWork.Complete();

            // NOTA: La generación del PDF se hará en la Fase 5
            // Por ahora solo devolvemos el mensaje de éxito

            return new ReleasePaymentResponse
            {
                Success = true,
                Message = "Pago liberado exitosamente",
                FreelancerAmount = freelancerAmount,
                CommissionAmount = commissionAmount,
                ReceiptUrl = null // Se generará en Fase 5
            };
        }
        catch (Exception ex)
        {
            return new ReleasePaymentResponse
            {
                Success = false,
                Message = $"Error al liberar pago: {ex.Message}"
            };
        }
    }
}
