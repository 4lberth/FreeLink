using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.CreateProposal;

public class CreateProposalHandler : IRequestHandler<CreateProposalCommand, CreateProposalResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateProposalHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CreateProposalResponse> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el usuario es freelancer
            var freelancer = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.FreelancerId);
            if (freelancer == null)
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "Freelancer no encontrado"
                };
            }

            if (freelancer.UserType != "Freelancer")
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "Solo los freelancers pueden crear propuestas"
                };
            }

            // 2. Validar que el requesting user es el mismo freelancer
            if (request.RequestingUserId != request.FreelancerId)
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "No puedes crear propuestas para otro freelancer"
                };
            }

            // 3. Verificar que el proyecto existe
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 4. Verificar que el proyecto está asignado al freelancer
            if (project.ProjectStatus != "Asignado" || project.AssignedFreelancerId != request.FreelancerId)
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "Solo puedes crear propuestas para proyectos asignados a ti"
                };
            }

            // 5. Verificar que no existe una propuesta aceptada para este proyecto
            var existingAcceptedProposal = await _unitOfWork.Repository<Proposal>()
                .GetFirstOrDefaultAsync(p => p.ProjectId == request.ProjectId && p.ProposalStatus == "Aceptada");

            if (existingAcceptedProposal != null)
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "Ya existe una propuesta aceptada para este proyecto"
                };
            }

            // 6. Validar que el total coincide con la suma del desglose
            var totalFromBreakdown = request.CostBreakdown.Sum(item => item.Amount);
            if (Math.Abs(request.TotalCost - totalFromBreakdown) > 0.01m)
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = $"El total ({request.TotalCost}) no coincide con la suma del desglose ({totalFromBreakdown})"
                };
            }

            // 7. Validar que hay al menos un item de desglose
            if (!request.CostBreakdown.Any())
            {
                return new CreateProposalResponse
                {
                    Success = false,
                    Message = "Debe incluir al menos un item en el desglose de costos"
                };
            }

            // 8. Crear la propuesta
            var proposal = new Proposal
            {
                ProjectId = request.ProjectId,
                FreelancerId = request.FreelancerId,
                VersionNumber = 1,
                TotalCost = request.TotalCost,
                ProposalStatus = "Enviada",  // Valores ENUM: Borrador, Enviada, En Negociación, Aceptada, Rechazada
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Proposal>().Add(proposal);
            await _unitOfWork.Complete();

            // 9. Crear desglose de costos
            foreach (var item in request.CostBreakdown)
            {
                var costItem = new Proposalcostbreakdown
                {
                    ProposalId = proposal.ProposalId,
                    ItemDescription = item.ItemDescription,
                    Amount = item.Amount,
                    ItemOrder = item.ItemOrder
                };
                await _unitOfWork.Repository<Proposalcostbreakdown>().Add(costItem);
            }

            // 10. Crear cronograma (timeline)
            foreach (var milestone in request.Timeline)
            {
                var timelineItem = new Proposaltimeline
                {
                    ProposalId = proposal.ProposalId,
                    MilestoneName = milestone.MilestoneName,
                    Description = milestone.Description,
                    EstimatedDuration = milestone.EstimatedDuration,
                    ItemOrder = milestone.ItemOrder
                };
                await _unitOfWork.Repository<Proposaltimeline>().Add(timelineItem);
            }

            // 11. Crear entregables (deliverables)
            foreach (var deliverable in request.Deliverables)
            {
                var deliverableItem = new Proposaldeliverable
                {
                    ProposalId = proposal.ProposalId,
                    DeliverableName = deliverable.DeliverableName,
                    Description = deliverable.Description,
                    ItemOrder = deliverable.ItemOrder
                };
                await _unitOfWork.Repository<Proposaldeliverable>().Add(deliverableItem);
            }

            await _unitOfWork.Complete();

            // 12. Registrar actividad en el proyecto
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.FreelancerId,
                activityType: "ProposalSubmitted",
                description: $"Propuesta v{proposal.VersionNumber} enviada por el freelancer. Monto total: ${proposal.TotalCost:N2}"
            );

            // 13. Notificar al cliente
            await _notificationService.CreateNotificationAsync(
                userId: project.ClientId,
                type: "Proposal",
                title: "Nueva propuesta recibida",
                message: $"El freelancer ha enviado una propuesta para el proyecto '{project.Title}'. Revisa los detalles y decide si aceptar o pedir cambios.",
                resourceType: "Proposal",
                resourceId: proposal.ProposalId
            );

            return new CreateProposalResponse
            {
                Success = true,
                Message = "Propuesta creada exitosamente",
                ProposalId = proposal.ProposalId,
                VersionNumber = proposal.VersionNumber ?? 1
            };
        }
        catch (Exception ex)
        {
            return new CreateProposalResponse
            {
                Success = false,
                Message = $"Error al crear propuesta: {ex.Message}"
            };
        }
    }
}
