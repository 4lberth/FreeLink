using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.SearchProjects;

public class SearchProjectsHandler : IRequestHandler<SearchProjectsQuery, SearchProjectsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchProjectsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SearchProjectsResponse> Handle(SearchProjectsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener todos los proyectos publicados
            var projects = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.ProjectStatus == "Publicado");

            // Aplicar filtros
            if (request.BudgetMin.HasValue)
            {
                projects = projects.Where(p => p.Budget >= request.BudgetMin.Value);
            }

            if (request.BudgetMax.HasValue)
            {
                projects = projects.Where(p => p.Budget <= request.BudgetMax.Value);
            }

            // Filtrar por skill si se especifica
            if (request.SkillId.HasValue)
            {
                var projectsWithSkill = await _unitOfWork.Repository<Projectskill>()
                    .GetAsync(ps => ps.SkillId == request.SkillId.Value);
                
                var projectIds = projectsWithSkill.Select(ps => ps.ProjectId).ToList();
                projects = projects.Where(p => projectIds.Contains(p.ProjectId));
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

            return new SearchProjectsResponse
            {
                Success = true,
                Message = $"{projectDtos.Count} proyectos encontrados",
                Projects = projectDtos
            };
        }
        catch (Exception ex)
        {
            return new SearchProjectsResponse
            {
                Success = false,
                Message = $"Error al buscar proyectos: {ex.Message}"
            };
        }
    }
}
