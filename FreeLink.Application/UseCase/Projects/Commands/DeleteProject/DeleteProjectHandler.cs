using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.DeleteProject;

public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, DeleteProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public DeleteProjectHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<DeleteProjectResponse> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new DeleteProjectResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Validar que el usuario autenticado es el cliente dueño
            if (project.ClientId != request.RequestingUserId)
            {
                return new DeleteProjectResponse
                {
                    Success = false,
                    Message = "No tienes permiso para eliminar este proyecto"
                };
            }

            // 3. Solo se puede eliminar si está "Publicado" (sin asignar)
            if (project.ProjectStatus != "Publicado")
            {
                return new DeleteProjectResponse
                {
                    Success = false,
                    Message = "Solo se pueden eliminar proyectos en estado 'Publicado'"
                };
            }

            // 4. Cambiar estado a "Cancelado" (soft delete)
            project.ProjectStatus = "Cancelado";
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Project>().Update(project);
            await _unitOfWork.Complete();

            // Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.RequestingUserId,
                activityType: "ProjectCancelled",
                description: $"Proyecto '{project.Title}' cancelado"
            );

            return new DeleteProjectResponse
            {
                Success = true,
                Message = "Proyecto cancelado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DeleteProjectResponse
            {
                Success = false,
                Message = $"Error al cancelar proyecto: {ex.Message}"
            };
        }
    }
}
