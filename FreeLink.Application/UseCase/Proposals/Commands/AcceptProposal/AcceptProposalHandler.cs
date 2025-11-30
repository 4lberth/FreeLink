using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.AcceptProposal;

public class AcceptProposalHandler : IRequestHandler<AcceptProposalCommand, AcceptProposalResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public AcceptProposalHandler(
        IUnitOfWork unitOfWork, 
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<AcceptProposalResponse> Handle(AcceptProposalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la propuesta
            var proposal = await _unitOfWork.Repository<Proposal>().GetById(request.ProposalId);
            if (proposal == null)
            {
                return new AcceptProposalResponse
                {
                    Success = false,
                    Message = "Propuesta no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(proposal.ProjectId);
            if (project == null)
            {
                return new AcceptProposalResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Validar que el usuario es el cliente dueño del proyecto
            if (project.ClientId != request.RequestingUserId)
            {
                return new AcceptProposalResponse
                {
                    Success = false,
                    Message = "Solo el cliente dueño del proyecto puede aceptar propuestas"
                };
            }

            // 4. Validar que la propuesta está en estado "Enviada" o "En Negociación"
            string currentStatus = string.IsNullOrWhiteSpace(proposal.ProposalStatus) ? "Enviada" : proposal.ProposalStatus;
            if (currentStatus != "Enviada" && currentStatus != "En Negociación")
            {
                return new AcceptProposalResponse
                {
                    Success = false,
                    Message = $"Solo se pueden aceptar propuestas en estado 'Enviada' o 'En Negociación'. Estado actual: '{currentStatus}'"
                };
            }

            // 5. Actualizar el estado de la propuesta
            proposal.ProposalStatus = "Aceptada";
            proposal.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Proposal>().Update(proposal);

            // 6. GENERAR CONTRATO AUTOMÁTICAMENTE
            var contract = new Contract
            {
                ProjectId = project.ProjectId,
                ProposalId = proposal.ProposalId,
                ClientId = project.ClientId,
                FreelancerId = proposal.FreelancerId,
                ContractStatus = "Pendiente Firma",  // ENUM: 'Pendiente Firma','Firmado','En Ejecución','Completado','Cancelado'
                TotalAmount = proposal.TotalCost,
                GeneratedAt = DateTime.UtcNow
                // ContractPdfUrl will be NULL until both parties sign
            };

            await _unitOfWork.Repository<Contract>().Add(contract);
            await _unitOfWork.Complete();

            // Note: PDF will be generated when both parties sign the contract (SignContractHandler)

            // 7. Registrar actividad en el proyecto
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ProposalAccepted",
                description: $"Propuesta v{proposal.VersionNumber} aceptada. Contrato generado automáticamente."
            );

            // 8. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: proposal.FreelancerId,
                type: "Proposal",
                title: "¡Propuesta aceptada!",
                message: $"El cliente ha aceptado tu propuesta para el proyecto '{project.Title}'. Se ha generado un contrato que debes firmar.",
                resourceType: "Contract",
                resourceId: contract.ContractId
            );

            // 9. Notificar al cliente sobre el contrato generado
            await _notificationService.CreateNotificationAsync(
                userId: project.ClientId,
                type: "Contract",
                title: "Contrato generado",
                message: $"El contrato para el proyecto '{project.Title}' ha sido generado. Revísalo y fírmalo para continuar.",
                resourceType: "Contract",
                resourceId: contract.ContractId
            );

            return new AcceptProposalResponse
            {
                Success = true,
                Message = "Propuesta aceptada exitosamente. Se ha generado un contrato automáticamente.",
                ContractId = contract.ContractId
            };
        }
        catch (Exception ex)
        {
            return new AcceptProposalResponse
            {
                Success = false,
                Message = $"Error al aceptar propuesta: {ex.Message}"
            };
        }
    }
}