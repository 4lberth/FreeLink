using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.RejectProposal;

public class RejectProposalHandler : IRequestHandler<RejectProposalCommand, RejectProposalResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RejectProposalHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<RejectProposalResponse> Handle(RejectProposalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la propuesta
            var proposal = await _unitOfWork.Repository<Proposal>().GetById(request.ProposalId);
            if (proposal == null)
            {
                return new RejectProposalResponse
                {
                    Success = false,
                    Message = "Propuesta no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(proposal.ProjectId);
            if (project == null)
            {
                return new RejectProposalResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Validar que el usuario es el cliente dueño del proyecto
            if (project.ClientId != request.RequestingUserId)
            {
                return new RejectProposalResponse
                {
                    Success = false,
                    Message = "Solo el cliente dueño del proyecto puede rechazar propuestas"
                };
            }

            // 4. Validar que la propuesta puede ser rechazada
            if (proposal.ProposalStatus == "Aceptada")
            {
                return new RejectProposalResponse
                {
                    Success = false,
                    Message = "No se puede rechazar una propuesta ya aceptada"
                };
            }

            if (proposal.ProposalStatus == "Rechazada")
            {
                return new RejectProposalResponse
                {
                    Success = false,
                    Message = "Esta propuesta ya fue rechazada anteriormente"
                };
            }

            // 5. Cambiar estado a "Rechazada"
            proposal.ProposalStatus = "Rechazada";
            proposal.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Proposal>().Update(proposal);

            // 6. Si hay razón, crear comentario
            if (!string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                var comment = new Proposalcomment
                {
                    ProposalId = proposal.ProposalId,
                    UserId = request.RequestingUserId,
                    CommentText = $"[PROPUESTA RECHAZADA] {request.RejectionReason}",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Proposalcomment>().Add(comment);
            }

            await _unitOfWork.Complete();

            // 7. Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ProposalRejected",
                description: $"Propuesta v{proposal.VersionNumber} rechazada por el cliente"
            );

            // 8. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: proposal.FreelancerId,
                type: "Proposal",
                title: "Propuesta rechazada",
                message: $"Tu propuesta para el proyecto '{project.Title}' ha sido rechazada por el cliente.{(!string.IsNullOrWhiteSpace(request.RejectionReason) ? " Revisa los comentarios para más detalles." : "")}",
                resourceType: "Proposal",
                resourceId: proposal.ProposalId
            );

            return new RejectProposalResponse
            {
                Success = true,
                Message = "Propuesta rechazada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new RejectProposalResponse
            {
                Success = false,
                Message = $"Error al rechazar propuesta: {ex.Message}"
            };
        }
    }
}
