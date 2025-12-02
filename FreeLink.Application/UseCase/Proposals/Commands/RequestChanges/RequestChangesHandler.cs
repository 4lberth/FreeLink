using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.RequestChanges;

public class RequestChangesHandler : IRequestHandler<RequestChangesCommand, RequestChangesResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RequestChangesHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<RequestChangesResponse> Handle(RequestChangesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la propuesta
            var proposal = await _unitOfWork.Repository<Proposal>().GetById(request.ProposalId);
            if (proposal == null)
            {
                return new RequestChangesResponse
                {
                    Success = false,
                    Message = "Propuesta no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(proposal.ProjectId);
            if (project == null)
            {
                return new RequestChangesResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Validar que el usuario es el cliente dueño del proyecto
            if (project.ClientId != request.RequestingUserId)
            {
                return new RequestChangesResponse
                {
                    Success = false,
                    Message = "Solo el cliente dueño del proyecto puede solicitar cambios"
                };
            }

            // 4. Validar que la propuesta está en estado "Enviada" (o vacío por propuestas antiguas)
            string currentStatus = string.IsNullOrWhiteSpace(proposal.ProposalStatus) ? "Enviada" : proposal.ProposalStatus;
            if (currentStatus != "Enviada")
            {
                return new RequestChangesResponse
                {
                    Success = false,
                    Message = $"Solo se pueden solicitar cambios en propuestas 'Enviada'. Estado actual: '{currentStatus}'"
                };
            }

            // 5. Cambiar estado a "En Negociación"
            proposal.ProposalStatus = "En Negociación";
            proposal.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Proposal>().Update(proposal);

            // 6. Crear comentario automático con la razón
            var comment = new Proposalcomment
            {
                ProposalId = proposal.ProposalId,
                UserId = request.RequestingUserId,
                CommentText = $"[CAMBIOS SOLICITADOS] {request.ChangeReason}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Proposalcomment>().Add(comment);
            await _unitOfWork.Complete();

            // 7. Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ProposalChangesRequested",
                description: $"Cliente solicitó cambios en propuesta v{proposal.VersionNumber}"
            );

            // 8. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: proposal.FreelancerId,
                type: "Proposal",
                title: "Cambios solicitados en tu propuesta",
                message: $"El cliente revisó tu propuesta para '{project.Title}' y ha solicitado algunos cambios. Revisa los comentarios y envía una nueva versión.",
                resourceType: "Proposal",
                resourceId: proposal.ProposalId
            );

            return new RequestChangesResponse
            {
                Success = true,
                Message = "Cambios solicitados exitosamente. El freelancer ha sido notificado."
            };
        }
        catch (Exception ex)
        {
            return new RequestChangesResponse
            {
                Success = false,
                Message = $"Error al solicitar cambios: {ex.Message}"
            };
        }
    }
}
