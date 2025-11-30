using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.AcceptApplication;

public class AcceptApplicationHandler : IRequestHandler<AcceptApplicationCommand, AcceptApplicationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public AcceptApplicationHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<AcceptApplicationResponse> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la aplicación
            var application = await _unitOfWork.Repository<Projectapplication>().GetById(request.ApplicationId);
            if (application == null)
            {
                return new AcceptApplicationResponse
                {
                    Success = false,
                    Message = "Aplicación no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(application.ProjectId);
            if (project == null)
            {
                return new AcceptApplicationResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Validar que el usuario autenticado es el cliente dueño del proyecto
            if (project.ClientId != request.RequestingUserId)
            {
                return new AcceptApplicationResponse
                {
                    Success = false,
                    Message = "No tienes permiso para aceptar aplicaciones de este proyecto"
                };
            }

            // 4. Validar que el proyecto está en estado "Publicado"
            if (project.ProjectStatus != "Publicado")
            {
                return new AcceptApplicationResponse
                {
                    Success = false,
                    Message = "Solo se pueden aceptar aplicaciones de proyectos en estado 'Publicado'"
                };
            }

            // 5. Validar que la aplicación está en estado "Pendiente"
            if (application.ApplicationStatus != "Pendiente")
            {
                return new AcceptApplicationResponse
                {
                    Success = false,
                    Message = "Solo se pueden aceptar aplicaciones en estado 'Pendiente'"
                };
            }

            // 6. Actualizar la aplicación a "Aceptada"
            application.ApplicationStatus = "Aceptada";
            application.RespondedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Projectapplication>().Update(application);

            // 7. Actualizar el proyecto a "Asignado" y asignar el freelancer
            project.ProjectStatus = "Asignado";
            project.AssignedFreelancerId = application.FreelancerId;
            project.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Project>().Update(project);

            // 8. Rechazar automáticamente todas las demás aplicaciones pendientes
            var otherApplications = await _unitOfWork.Repository<Projectapplication>()
                .GetAsync(pa => pa.ProjectId == application.ProjectId 
                    && pa.ApplicationId != request.ApplicationId 
                    && pa.ApplicationStatus == "Pendiente");

            foreach (var otherApp in otherApplications)
            {
                otherApp.ApplicationStatus = "Rechazada";
                otherApp.RespondedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Projectapplication>().Update(otherApp);
            }

            await _unitOfWork.Complete();

            // Registrar actividad de aceptación
            await _notificationService.LogProjectActivityAsync(
                projectId: application.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ApplicationAccepted",
                description: $"Aplicación aceptada. Proyecto asignado al freelancer ID {application.FreelancerId}"
            );

            // Registrar actividades de rechazo automático
            foreach (var otherApp in otherApplications)
            {
                await _notificationService.LogProjectActivityAsync(
                    projectId: application.ProjectId,
                    userId: null,
                    activityType: "ApplicationAutoRejected",
                    description: $"Aplicación del freelancer ID {otherApp.FreelancerId} rechazada automáticamente"
                );
            }

            // Notificar al freelancer aceptado
            await _notificationService.CreateNotificationAsync(
                userId: application.FreelancerId,
                type: "Application",
                title: "¡Tu aplicación fue aceptada!",
                message: $"El cliente aceptó tu aplicación para el proyecto '{project.Title}'. Ya puedes iniciar el trabajo.",
                resourceType: "Project",
                resourceId: project.ProjectId
            );

            // Notificar a los freelancers rechazados automáticamente
            if (otherApplications.Any())
            {
                var rejectedFreelancerIds = otherApplications.Select(a => a.FreelancerId).ToList();
                await _notificationService.CreateMultipleNotificationsAsync(
                    userIds: rejectedFreelancerIds,
                    type: "Application",
                    title: "Aplicación rechazada",
                    message: $"Tu aplicación para el proyecto '{project.Title}' no fue seleccionada.",
                    resourceType: "Project",
                    resourceId: project.ProjectId
                );
            }

            return new AcceptApplicationResponse
            {
                Success = true,
                Message = "Aplicación aceptada exitosamente. El proyecto ha sido asignado al freelancer."
            };
        }
        catch (Exception ex)
        {
            return new AcceptApplicationResponse
            {
                Success = false,
                Message = $"Error al aceptar aplicación: {ex.Message}"
            };
        }
    }
}
