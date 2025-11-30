using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.RejectApplication;

public class RejectApplicationHandler : IRequestHandler<RejectApplicationCommand, RejectApplicationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RejectApplicationHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<RejectApplicationResponse> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la aplicación
            var application = await _unitOfWork.Repository<Projectapplication>().GetById(request.ApplicationId);
            if (application == null)
            {
                return new RejectApplicationResponse
                {
                    Success = false,
                    Message = "Aplicación no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(application.ProjectId);
            if (project == null)
            {
                return new RejectApplicationResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Validar que el usuario autenticado es el cliente dueño del proyecto
            if (project.ClientId != request.RequestingUserId)
            {
                return new RejectApplicationResponse
                {
                    Success = false,
                    Message = "No tienes permiso para rechazar aplicaciones de este proyecto"
                };
            }

            // 4. Validar que la aplicación está en estado "Pendiente"
            if (application.ApplicationStatus != "Pendiente")
            {
                return new RejectApplicationResponse
                {
                    Success = false,
                    Message = "Solo se pueden rechazar aplicaciones en estado 'Pendiente'"
                };
            }

            // 5. Actualizar la aplicación a "Rechazada"
            application.ApplicationStatus = "Rechazada";
            application.RespondedAt = DateTime.UtcNow;
            
            await _unitOfWork.Repository<Projectapplication>().Update(application);
            await _unitOfWork.Complete();

            // Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: application.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ApplicationRejected",
                description: $"Aplicación del freelancer ID {application.FreelancerId} rechazada"
            );

            return new RejectApplicationResponse
            {
                Success = true,
                Message = "Aplicación rechazada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new RejectApplicationResponse
            {
                Success = false,
                Message = $"Error al rechazar aplicación: {ex.Message}"
            };
        }
    }
}
