using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectById;

public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, GetProjectByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectByIdResponse> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectByIdResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // Solo el cliente dueño puede ver proyectos cancelados
            if (project.ProjectStatus == "Cancelado" && 
                (!request.RequestingUserId.HasValue || request.RequestingUserId.Value != project.ClientId))
            {
                return new GetProjectByIdResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // Obtener cliente
            var client = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(project.ClientId);
            var clientProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(up => up.UserId == project.ClientId);

            // Obtener freelancer asignado (si existe)
            string? freelancerName = null;
            if (project.AssignedFreelancerId.HasValue)
            {
                var freelancerProfile = await _unitOfWork.Repository<Userprofile>()
                    .GetFirstOrDefaultAsync(up => up.UserId == project.AssignedFreelancerId.Value);
                freelancerName = freelancerProfile != null
                    ? $"{freelancerProfile.FirstName} {freelancerProfile.LastName}"
                    : null;
            }

            // Obtener skills requeridas
            var projectSkills = await _unitOfWork.Repository<Projectskill>()
                .GetAsync(ps => ps.ProjectId == request.ProjectId);
            
            var skills = new List<SkillDto>();
            foreach (var ps in projectSkills)
            {
                var skill = await _unitOfWork.Repository<Skill>().GetById(ps.SkillId);
                if (skill != null)
                {
                    skills.Add(new SkillDto
                    {
                        SkillId = skill.SkillId,
                        SkillName = skill.SkillName ?? string.Empty,
                        Category = skill.Category
                    });
                }
            }

            // Contar aplicaciones (solo si es el cliente dueño)
            int? applicationCount = null;
            if (request.RequestingUserId.HasValue && request.RequestingUserId.Value == project.ClientId)
            {
                var applications = await _unitOfWork.Repository<Projectapplication>()
                    .GetAsync(pa => pa.ProjectId == request.ProjectId);
                applicationCount = applications.Count();
            }

            var projectDto = new ProjectDto
            {
                ProjectId = project.ProjectId,
                Title = project.Title,
                Description = project.Description,
                Budget = project.Budget,
                DeadlineDate = project.DeadlineDate,
                ProjectStatus = project.ProjectStatus ?? "Publicado",
                CreatedAt = project.CreatedAt,
                StartDate = project.StartDate,
                CompletionDate = project.CompletionDate,
                ClientId = project.ClientId,
                ClientName = clientProfile != null 
                    ? $"{clientProfile.FirstName} {clientProfile.LastName}" 
                    : client?.Email ?? "Cliente",
                AssignedFreelancerId = project.AssignedFreelancerId,
                AssignedFreelancerName = freelancerName,
                RequiredSkills = skills,
                ApplicationCount = applicationCount
            };

            return new GetProjectByIdResponse
            {
                Success = true,
                Message = "Proyecto obtenido exitosamente",
                Project = projectDto
            };
        }
        catch (Exception ex)
        {
            return new GetProjectByIdResponse
            {
                Success = false,
                Message = $"Error al obtener proyecto: {ex.Message}"
            };
        }
    }
}
