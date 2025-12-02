using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.RejectDeliverable;

public class RejectDeliverableHandler : IRequestHandler<RejectDeliverableCommand, RejectDeliverableResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RejectDeliverableHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<RejectDeliverableResponse> Handle(RejectDeliverableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comentarios obligatorios
            if (string.IsNullOrWhiteSpace(request.RejectionComments))
            {
                return new RejectDeliverableResponse
                {
                    Success = false,
                    Message = "Los comentarios de rechazo son obligatorios"
                };
            }

            // 2. Obtener entregable
            var deliverable = await _unitOfWork.Repository<Projectdeliverable>().GetById(request.DeliverableId);
            if (deliverable == null)
            {
                return new RejectDeliverableResponse
                {
                    Success = false,
                    Message = "Entregable no encontrado"
                };
            }

            // 3. Obtener proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(deliverable.ProjectId);
            if (project == null)
            {
                return new RejectDeliverableResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 4. Verificar que el usuario es el cliente
            if (project.ClientId != request.ClientId)
            {
                return new RejectDeliverableResponse
                {
                    Success = false,
                    Message = "Solo el cliente puede rechazar entregables"
                };
            }

            // 5. Verificar estado
            if (deliverable.DeliverableStatus != "En Revisión")
            {
                return new RejectDeliverableResponse
                {
                    Success = false,
                    Message = "Solo se pueden rechazar entregables en revisión"
                };
            }

            // 6. Rechazar entregable
            deliverable.DeliverableStatus = "Rechazado";
            deliverable.ReviewedAt = DateTime.UtcNow;
            deliverable.ReviewComments = request.RejectionComments;

            await _unitOfWork.Repository<Projectdeliverable>().Update(deliverable);

            // 7. Activity log
            var activityLog = new Projectactivitylog
            {
                ProjectId = deliverable.ProjectId,
                UserId = request.ClientId,
                ActivityType = "DeliverableRejected",
                ActivityDescription = $"Entregable rechazado: {deliverable.Title}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Projectactivitylog>().Add(activityLog);

            // 8. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: project.AssignedFreelancerId!.Value,
                type: "DeliverableRejected",
                title: "Entregable rechazado",
                message: $"Tu entregable ha sido rechazado: {deliverable.Title}. Revisa los comentarios.",
                resourceType: "Deliverable",
                resourceId: deliverable.DeliverableId
            );

            await _unitOfWork.Complete();

            return new RejectDeliverableResponse
            {
                Success = true,
                Message = "Entregable rechazado. El freelancer ha sido notificado."
            };
        }
        catch (Exception ex)
        {
            return new RejectDeliverableResponse
            {
                Success = false,
                Message = $"Error al rechazar entregable: {ex.Message}"
            };
        }
    }
}
