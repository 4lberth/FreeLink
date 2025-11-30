using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.UpdateProject;

public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, UpdateProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new UpdateProjectResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Validar que el usuario autenticado es el cliente dueño
            if (project.ClientId != request.RequestingUserId)
            {
                return new UpdateProjectResponse
                {
                    Success = false,
                    Message = "No tienes permiso para editar este proyecto"
                };
            }

            // 3. Solo se puede editar si está "Publicado"
            if (project.ProjectStatus != "Publicado")
            {
                return new UpdateProjectResponse
                {
                    Success = false,
                    Message = "Solo se pueden editar proyectos en estado 'Publicado'"
                };
            }

            // 4. Validaciones de negocio
            if (request.Budget <= 0)
            {
                return new UpdateProjectResponse
                {
                    Success = false,
                    Message = "El presupuesto debe ser mayor a 0"
                };
            }

            if (request.DeadlineDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                return new UpdateProjectResponse
                {
                    Success = false,
                    Message = "La fecha límite debe ser futura"
                };
            }

            // 5. Verificar que las skills existen
            if (request.RequiredSkillIds.Any())
            {
                var skills = await _unitOfWork.Repository<Skill>().GetAll();
                var validSkillIds = skills.Select(s => s.SkillId).ToList();
                
                var invalidSkills = request.RequiredSkillIds.Except(validSkillIds).ToList();
                if (invalidSkills.Any())
                {
                    return new UpdateProjectResponse
                    {
                        Success = false,
                        Message = $"Las siguientes skills no existen: {string.Join(", ", invalidSkills)}"
                    };
                }
            }

            // 6. Actualizar el proyecto
            project.Title = request.Title;
            project.Description = request.Description;
            project.Budget = request.Budget;
            project.DeadlineDate = request.DeadlineDate;
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Project>().Update(project);

            // 7. Actualizar skills requeridas (eliminar y recrear)
            var existingSkills = await _unitOfWork.Repository<Projectskill>()
                .GetAsync(ps => ps.ProjectId == request.ProjectId);

            foreach (var skill in existingSkills)
            {
                await _unitOfWork.Repository<Projectskill>().Delete(skill.ProjectSkillId);
            }

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

            return new UpdateProjectResponse
            {
                Success = true,
                Message = "Proyecto actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateProjectResponse
            {
                Success = false,
                Message = $"Error al actualizar proyecto: {ex.Message}"
            };
        }
    }
}
