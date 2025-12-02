using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.CreateNewProposalVersion;

public class CreateNewProposalVersionHandler : IRequestHandler<CreateNewProposalVersionCommand, CreateNewProposalVersionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateNewProposalVersionHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CreateNewProposalVersionResponse> Handle(CreateNewProposalVersionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar freelancer
            var freelancer = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.FreelancerId);
            if (freelancer == null || freelancer.UserType != "Freelancer")
            {
                return new CreateNewProposalVersionResponse
                {
                    Success = false,
                    Message = "Solo los freelancers pueden crear propuestas"
                };
            }

            if (request.RequestingUserId != request.FreelancerId)
            {
                return new CreateNewProposalVersionResponse
                {
                    Success = false,
                    Message = "No puedes crear propuestas para otro freelancer"
                };
            }

            // 2. Buscar proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new CreateNewProposalVersionResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Buscar última versión de la propuesta
            var lastProposal = await _unitOfWork.Repository<Proposal>()
                .GetAsync(p => p.ProjectId == request.ProjectId && p.FreelancerId == request.FreelancerId);

            var lastVersion = lastProposal.OrderByDescending(p => p.VersionNumber).FirstOrDefault();

            if (lastVersion == null)
            {
                return new CreateNewProposalVersionResponse
                {
                    Success = false,
                    Message = "No existe una propuesta previa. Usa CreateProposal para la primera versión"
                };
            }

            // 4. Validar que está en estado "En Negociación"
            string currentStatus = string.IsNullOrWhiteSpace(lastVersion.ProposalStatus) ? "Enviada" : lastVersion.ProposalStatus;
            if (currentStatus != "En Negociación")
            {
                return new CreateNewProposalVersionResponse
                {
                    Success = false,
                    Message = $"Solo puedes crear nueva versión si la propuesta está 'En Negociación'. Estado actual: '{currentStatus}'"
                };
            }

            // 5. Validar total = suma
            var totalFromBreakdown = request.CostBreakdown.Sum(item => item.Amount);
            if (Math.Abs(request.TotalCost - totalFromBreakdown) > 0.01m)
            {
                return new CreateNewProposalVersionResponse
                {
                    Success = false,
                    Message = $"El total ({request.TotalCost}) no coincide con la suma ({totalFromBreakdown})"
                };
            }

            // 6. Crear nueva versión
            int newVersionNumber = (lastVersion.VersionNumber ?? 1) + 1;

            var newProposal = new Proposal
            {
                ProjectId = request.ProjectId,
                FreelancerId = request.FreelancerId,
                VersionNumber = newVersionNumber,
                TotalCost = request.TotalCost,
                ProposalStatus = "Enviada",  // ✅ ENUM: 'Borrador','Enviada','En Negociación','Aceptada','Rechazada'
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Proposal>().Add(newProposal);
            await _unitOfWork.Complete();

            // 7. Crear desglose
            foreach (var item in request.CostBreakdown)
            {
                await _unitOfWork.Repository<Proposalcostbreakdown>().Add(new()
                {
                    ProposalId = newProposal.ProposalId,
                    ItemDescription = item.ItemDescription,
                    Amount = item.Amount,
                    ItemOrder = item.ItemOrder
                });
            }

            // 8. Crear cronograma
            foreach (var milestone in request.Timeline)
            {
                await _unitOfWork.Repository<Proposaltimeline>().Add(new()
                {
                    ProposalId = newProposal.ProposalId,
                    MilestoneName = milestone.MilestoneName,
                    Description = milestone.Description,
                    EstimatedDuration = milestone.EstimatedDuration,
                    ItemOrder = milestone.ItemOrder
                });
            }

            // 9. Crear entregables
            foreach (var deliverable in request.Deliverables)
            {
                await _unitOfWork.Repository<Proposaldeliverable>().Add(new()
                {
                    ProposalId = newProposal.ProposalId,
                    DeliverableName = deliverable.DeliverableName,
                    Description = deliverable.Description,
                    ItemOrder = deliverable.ItemOrder
                });
            }

            await _unitOfWork.Complete();

            // 10. Comentario automático si hay razón
            if (!string.IsNullOrWhiteSpace(request.ChangeReason))
            {
                await _unitOfWork.Repository<Proposalcomment>().Add(new()
                {
                    ProposalId = newProposal.ProposalId,
                    UserId = request.FreelancerId,
                    CommentText = $"[NUEVA VERSIÓN v{newVersionNumber}] {request.ChangeReason}",
                    CreatedAt = DateTime.UtcNow
                });
                await _unitOfWork.Complete();
            }

            // 11. Activity log
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.FreelancerId,
                activityType: "ProposalNewVersionCreated",
                description: $"Nueva versión v{newVersionNumber} de propuesta enviada. Monto total: ${newProposal.TotalCost:N2}"
            );

            // 12. Notificar cliente
            await _notificationService.CreateNotificationAsync(
                userId: project.ClientId,
                type: "Proposal",
                title: "Nueva versión de propuesta recibida",
                message: $"El freelancer envió la versión {newVersionNumber} de la propuesta para '{project.Title}'. Revisa los cambios realizados.",
                resourceType: "Proposal",
                resourceId: newProposal.ProposalId
            );

            return new CreateNewProposalVersionResponse
            {
                Success = true,
                Message = $"Nueva versión v{newVersionNumber} creada exitosamente",
                ProposalId = newProposal.ProposalId,
                VersionNumber = newVersionNumber
            };
        }
        catch (Exception ex)
        {
            return new CreateNewProposalVersionResponse
            {
                Success = false,
                Message = $"Error al crear nueva versión: {ex.Message}"
            };
        }
    }
}
