using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.StartProject;

public class StartProjectHandler : IRequestHandler<StartProjectCommand, StartProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public StartProjectHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<StartProjectResponse> Handle(StartProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new StartProjectResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // Validar que el usuario es el cliente o el freelancer asignado
            if (project.ClientId != request.RequestingUserId && 
                project.AssignedFreelancerId != request.RequestingUserId)
            {
                return new StartProjectResponse
                {
                    Success = false,
                    Message = "No tienes permiso para iniciar este proyecto"
                };
            }

            // Validar que el proyecto está en estado "Asignado"
            if (project.ProjectStatus != "Asignado")
            {
                return new StartProjectResponse
                {
                    Success = false,
                    Message = "Solo se pueden iniciar proyectos en estado 'Asignado'"
                };
            }

            project.ProjectStatus = "En Proceso";
            project.StartDate = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Project>().Update(project);
            await _unitOfWork.Complete();

            // Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ProjectStarted",
                description: $"Proyecto '{project.Title}' iniciado"
            );

            return new StartProjectResponse
            {
                Success = true,
                Message = "Proyecto iniciado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new StartProjectResponse
            {
                Success = false,
                Message = $"Error al iniciar proyecto: {ex.Message}"
            };
        }
    }
}
