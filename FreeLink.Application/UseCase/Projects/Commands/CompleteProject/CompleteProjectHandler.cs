using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.CompleteProject;

public class CompleteProjectHandler : IRequestHandler<CompleteProjectCommand, CompleteProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CompleteProjectHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CompleteProjectResponse> Handle(CompleteProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new CompleteProjectResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // Validar que el usuario es el cliente o el freelancer asignado
            if (project.ClientId != request.RequestingUserId && 
                project.AssignedFreelancerId != request.RequestingUserId)
            {
                return new CompleteProjectResponse
                {
                    Success = false,
                    Message = "No tienes permiso para completar este proyecto"
                };
            }

            // Validar que el proyecto está en estado "En Proceso"
            if (project.ProjectStatus != "En Proceso")
            {
                return new CompleteProjectResponse
                {
                    Success = false,
                    Message = "Solo se pueden completar proyectos en estado 'En Proceso'"
                };
            }

            project.ProjectStatus = "Completado";
            project.CompletionDate = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Project>().Update(project);
            await _unitOfWork.Complete();

            // Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ProjectCompleted",
                description: $"Proyecto '{project.Title}' marcado como completado"
            );

            // Notificar a cliente y freelancer
            var userIds = new List<int>();
            if (project.ClientId != request.RequestingUserId)
                userIds.Add(project.ClientId);
            if (project.AssignedFreelancerId.HasValue && project.AssignedFreelancerId != request.RequestingUserId)
                userIds.Add(project.AssignedFreelancerId.Value);

            if (userIds.Any())
            {
                await _notificationService.CreateMultipleNotificationsAsync(
                    userIds: userIds,
                    type: "Project",
                    title: "Proyecto completado",
                    message: $"El proyecto '{project.Title}' ha sido marcado como completado. Ya puedes dejar una review.",
                    resourceType: "Project",
                    resourceId: project.ProjectId
                );
            }

            return new CompleteProjectResponse
            {
                Success = true,
                Message = "Proyecto completado exitosamente. Ahora puedes dejar una review."
            };
        }
        catch (Exception ex)
        {
            return new CompleteProjectResponse
            {
                Success = false,
                Message = $"Error al completar proyecto: {ex.Message}"
            };
        }
    }
}
