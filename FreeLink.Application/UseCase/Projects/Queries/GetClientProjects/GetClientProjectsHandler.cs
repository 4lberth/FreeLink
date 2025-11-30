using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetClientProjects;

public class GetClientProjectsHandler : IRequestHandler<GetClientProjectsQuery, GetClientProjectsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientProjectsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetClientProjectsResponse> Handle(GetClientProjectsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener todos los proyectos del cliente
            var projects = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.ClientId == request.ClientId);

            // Aplicar filtro de estado si se especifica
            if (!string.IsNullOrEmpty(request.StatusFilter))
            {
                projects = projects.Where(p => p.ProjectStatus == request.StatusFilter);
            }

            var projectDtos = new List<ProjectSummaryDto>();

            foreach (var project in projects.OrderByDescending(p => p.CreatedAt))
            {
                // Obtener cliente
                var clientProfile = await _unitOfWork.Repository<Userprofile>()
                    .GetFirstOrDefaultAsync(up => up.UserId == project.ClientId);

                // Obtener skills
                var projectSkills = await _unitOfWork.Repository<Projectskill>()
                    .GetAsync(ps => ps.ProjectId == project.ProjectId);

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

                projectDtos.Add(new ProjectSummaryDto
                {
                    ProjectId = project.ProjectId,
                    Title = project.Title,
                    Description = project.Description,
                    Budget = project.Budget,
                    DeadlineDate = project.DeadlineDate,
                    ProjectStatus = project.ProjectStatus ?? "Publicado",
                    ClientName = clientProfile != null 
                        ? $"{clientProfile.FirstName} {clientProfile.LastName}" 
                        : "Cliente",
                    RequiredSkills = skills
                });
            }

            return new GetClientProjectsResponse
            {
                Success = true,
                Message = $"{projectDtos.Count} proyectos encontrados",
                Projects = projectDtos
            };
        }
        catch (Exception ex)
        {
            return new GetClientProjectsResponse
            {
                Success = false,
                Message = $"Error al obtener proyectos: {ex.Message}"
            };
        }
    }
}
