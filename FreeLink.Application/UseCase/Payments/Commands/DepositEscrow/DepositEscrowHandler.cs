using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.DepositEscrow;

public class DepositEscrowHandler : IRequestHandler<DepositEscrowCommand, DepositEscrowResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IWalletService _walletService;

    public DepositEscrowHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IWalletService walletService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _walletService = walletService;
    }

    public async Task<DepositEscrowResponse> Handle(DepositEscrowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new DepositEscrowResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Verificar que el usuario es el cliente del proyecto
            if (project.ClientId != request.ClientId)
            {
                return new DepositEscrowResponse
                {
                    Success = false,
                    Message = "Solo el cliente del proyecto puede depositar fondos en garantía"
                };
            }

            // 3. Verificar que el proyecto tiene freelancer asignado
            if (!project.AssignedFreelancerId.HasValue)
            {
                return new DepositEscrowResponse
                {
                    Success = false,
                    Message = "El proyecto debe tener un freelancer asignado antes de depositar fondos"
                };
            }

            // 4. Validar que no exista ya un escrow para este proyecto
            var existingEscrowQuery = await _unitOfWork.Repository<Escrowaccount>()
                .GetAsync(e => e.ProjectId == request.ProjectId);
            var existingEscrow = existingEscrowQuery.FirstOrDefault();

            if (existingEscrow != null)
            {
                return new DepositEscrowResponse
                {
                    Success = false,
                    Message = "Ya existe una cuenta de garantía para este proyecto"
                };
            }

            // 5. Validar monto
            if (request.Amount <= 0)
            {
                return new DepositEscrowResponse
                {
                    Success = false,
                    Message = "El monto debe ser mayor a cero"
                };
            }

            // Verificar que el monto coincide con el presupuesto del proyecto
            if (request.Amount != project.Budget)
            {
                return new DepositEscrowResponse
                {
                    Success = false,
                    Message = $"El monto debe coincidir con el presupuesto del proyecto (${project.Budget})"
                };
            }

            // 6. Crear cuenta de garantía (escrow)
            var escrow = new Escrowaccount
            {
                ProjectId = request.ProjectId,
                ClientId = request.ClientId,
                FreelancerId = project.AssignedFreelancerId.Value,
                TotalAmount = request.Amount,
                EscrowStatus = "Deposited", // Estado: Depositado
                DepositedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Escrowaccount>().Add(escrow);
            await _unitOfWork.Complete(); // Guardar para obtener EscrowId

            // 7. Registrar transacción de depósito
            var transaction = new Transaction
            {
                EscrowId = escrow.EscrowId,
                FromUserId = request.ClientId,
                ToUserId = null, // El dinero va al escrow, no a un usuario específico
                Amount = request.Amount,
                TransactionType = "Depósito",
                TransactionStatus = "Completada",
                Description = $"Depósito en garantía para proyecto: {project.Title}",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Transaction>().Add(transaction);

            // 8. Actualizar wallet del cliente
            await _walletService.UpdateWalletForTransactionAsync(
                userId: request.ClientId,
                transactionType: "Depósito",
                amount: request.Amount,
                transactionStatus: "Completada",
                isFromUser: true // El cliente es FromUserId
            );

            // 9. Registrar actividad en el proyecto
            var activityLog = new Projectactivitylog
            {
                ProjectId = request.ProjectId,
                UserId = request.ClientId,
                ActivityType = "EscrowDeposited",
                ActivityDescription = $"Fondos depositados en garantía: ${request.Amount}",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Projectactivitylog>().Add(activityLog);

            // 10. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: project.AssignedFreelancerId.Value,
                type: "EscrowDeposited",
                title: "Fondos depositados en garantía",
                message: $"El cliente ha depositado ${request.Amount:F2} en garantía. Puedes comenzar a trabajar en el proyecto.",
                resourceType: "Escrow",
                resourceId: escrow.EscrowId
            );

            await _unitOfWork.Complete();

            return new DepositEscrowResponse
            {
                Success = true,
                Message = "Fondos depositados exitosamente en garantía",
                EscrowId = escrow.EscrowId,
                Amount = escrow.TotalAmount
            };
        }
        catch (Exception ex)
        {
            return new DepositEscrowResponse
            {
                Success = false,
                Message = $"Error al depositar fondos: {ex.Message}"
            };
        }
    }
}
