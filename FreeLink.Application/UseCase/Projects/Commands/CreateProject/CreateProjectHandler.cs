using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.CreateProject;

public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, CreateProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateProjectHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CreateProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el usuario existe y es un cliente
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.ClientId);
            if (user == null)
            {
                return new CreateProjectResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            if (user.UserType != "Cliente")
            {
                return new CreateProjectResponse
                {
                    Success = false,
                    Message = "Solo los clientes pueden crear proyectos"
                };
            }

            // 2. Validar que el usuario autenticado es el mismo que el cliente
            if (request.RequestingUserId != request.ClientId)
            {
                return new CreateProjectResponse
                {
                    Success = false,
                    Message = "No tienes permiso para crear proyectos para otro usuario"
                };
            }

            // 3. Validaciones de negocio
            if (request.Budget <= 0)
            {
                return new CreateProjectResponse
                {
                    Success = false,
                    Message = "El presupuesto debe ser mayor a 0"
                };
            }

            if (request.DeadlineDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                return new CreateProjectResponse
                {
                    Success = false,
                    Message = "La fecha límite debe ser futura"
                };
            }

            // 4. Verificar que las skills existen
            if (request.RequiredSkillIds.Any())
            {
                var skills = await _unitOfWork.Repository<Skill>().GetAll();
                var validSkillIds = skills.Select(s => s.SkillId).ToList();
                
                var invalidSkills = request.RequiredSkillIds.Except(validSkillIds).ToList();
                if (invalidSkills.Any())
                {
                    return new CreateProjectResponse
                    {
                        Success = false,
                        Message = $"Las siguientes skills no existen: {string.Join(", ", invalidSkills)}"
                    };
                }
            }

            // 5. Crear el proyecto
            var project = new Project
            {
                ClientId = request.ClientId,
                Title = request.Title,
                Description = request.Description,
                Budget = request.Budget,
                DeadlineDate = request.DeadlineDate,
                ProjectStatus = "Publicado",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Project>().Add(project);
            await _unitOfWork.Complete();

            // 6. Asignar skills requeridas
            if (request.RequiredSkillIds.Any())
            {
                foreach (var skillId in request.RequiredSkillIds)
                {
                    var projectSkill = new Projectskill
                    {
                        ProjectId = project.ProjectId,
                        SkillId = skillId
                    };
                    await _unitOfWork.Repository<Projectskill>().Add(projectSkill);
                }
                await _unitOfWork.Complete();
            }

            // Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: project.ProjectId,
                userId: request.ClientId,
                activityType: "ProjectCreated",
                description: $"Proyecto '{project.Title}' creado"
            );

            return new CreateProjectResponse
            {
                Success = true,
                Message = "Proyecto creado exitosamente",
                ProjectId = project.ProjectId
            };
        }
        catch (Exception ex)
        {
            return new CreateProjectResponse
            {
                Success = false,
                Message = $"Error al crear proyecto: {ex.Message}"
            };
        }
    }
}
