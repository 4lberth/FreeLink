using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.SubmitDeliverable;

public class SubmitDeliverableHandler : IRequestHandler<SubmitDeliverableCommand, SubmitDeliverableResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public SubmitDeliverableHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<SubmitDeliverableResponse> Handle(SubmitDeliverableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener entregable
            var deliverable = await _unitOfWork.Repository<Projectdeliverable>().GetById(request.DeliverableId);
            if (deliverable == null)
            {
                return new SubmitDeliverableResponse
                {
                    Success = false,
                    Message = "Entregable no encontrado"
                };
            }

            // 2. Obtener proyecto para verificar permisos
            var project = await _unitOfWork.Repository<Project>().GetById(deliverable.ProjectId);
            if (project == null)
            {
                return new SubmitDeliverableResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Verificar que el usuario es el freelancer
            if (project.AssignedFreelancerId != request.FreelancerId)
            {
                return new SubmitDeliverableResponse
                {
                    Success = false,
                    Message = "Solo el freelancer puede enviar entregables a revisión"
                };
            }

            // 4. Verificar estado actual
            if (deliverable.DeliverableStatus != "Pendiente")
            {
                return new SubmitDeliverableResponse
                {
                    Success = false,
                    Message = "Solo se pueden enviar entregables en estado Pendiente"
                };
            }

            // 5. Actualizar estado
            deliverable.DeliverableStatus = "En Revisión";
            deliverable.SubmittedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Projectdeliverable>().Update(deliverable);

            // 6. Activity log
            var activityLog = new Projectactivitylog
            {
                ProjectId = deliverable.ProjectId,
                UserId = request.FreelancerId,
                ActivityType = "DeliverableSubmitted",
                ActivityDescription = $"Entregable enviado a revisión: {deliverable.Title}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Projectactivitylog>().Add(activityLog);

            // 7. Not ificar al cliente
            await _notificationService.CreateNotificationAsync(
                userId: project.ClientId,
                type: "DeliverableSubmitted",
                title: "Entregable para revisión",
                message: $"El freelancer ha enviado un entregable para revisión: {deliverable.Title}",
                resourceType: "Deliverable",
                resourceId: deliverable.DeliverableId
            );

            await _unitOfWork.Complete();

            return new SubmitDeliverableResponse
            {
                Success = true,
                Message = "Entregable enviado a revisión exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new SubmitDeliverableResponse
            {
                Success = false, Message = $"Error al enviar entregable: {ex.Message}"
            };
        }
    }
}
