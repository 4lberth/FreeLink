using FreeLink.Application.UseCase.Projects.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.SearchProjects;

public class SearchProjectsQuery : IRequest<SearchProjectsResponse>
{
    public int? SkillId { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public string? Status { get; set; }
}
