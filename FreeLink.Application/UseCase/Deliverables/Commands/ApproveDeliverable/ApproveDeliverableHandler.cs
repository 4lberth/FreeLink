using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.ApproveDeliverable;

public class ApproveDeliverableHandler : IRequestHandler<ApproveDeliverableCommand, ApproveDeliverableResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public ApproveDeliverableHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<ApproveDeliverableResponse> Handle(ApproveDeliverableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener entregable
            var deliverable = await _unitOfWork.Repository<Projectdeliverable>().GetById(request.DeliverableId);
            if (deliverable == null)
            {
                return new ApproveDeliverableResponse
                {
                    Success = false,
                    Message = "Entregable no encontrado"
                };
            }

            // 2. Obtener proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(deliverable.ProjectId);
            if (project == null)
            {
                return new ApproveDeliverableResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Verificar que el usuario es el cliente
            if (project.ClientId != request.ClientId)
            {
                return new ApproveDeliverableResponse
                {
                    Success = false,
                    Message = "Solo el cliente puede aprobar entregables"
                };
            }

            // 4. Verificar estado
            if (deliverable.DeliverableStatus != "En Revisión")
            {
                return new ApproveDeliverableResponse
                {
                    Success = false,
                    Message = "Solo se pueden aprobar entregables en revisión"
                };
            }

            // 5. Aprobar entregable
            deliverable.DeliverableStatus = "Aprobado";
            deliverable.ReviewedAt = DateTime.UtcNow;
            deliverable.ReviewComments = request.Comments;

            await _unitOfWork.Repository<Projectdeliverable>().Update(deliverable);

            // 6. Activity log
            var activityLog = new Projectactivitylog
            {
                ProjectId = deliverable.ProjectId,
                UserId = request.ClientId,
                ActivityType = "DeliverableApproved",
                ActivityDescription = $"Entregable aprobado: {deliverable.Title}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Projectactivitylog>().Add(activityLog);

            // 7. Notificar al freelancer
            await _notificationService.CreateNotificationAsync(
                userId: project.AssignedFreelancerId!.Value,
                type: "DeliverableApproved",
                title: "Entregable aprobado",
                message: $"Tu entregable ha sido aprobado: {deliverable.Title}",
                resourceType: "Deliverable",
                resourceId: deliverable.DeliverableId
            );

            await _unitOfWork.Complete();

            return new ApproveDeliverableResponse
            {
                Success = true,
                Message = "Entregable aprobado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApproveDeliverableResponse
            {
                Success = false,
                Message = $"Error al aprobar entregable: {ex.Message}"
            };
        }
    }
}
