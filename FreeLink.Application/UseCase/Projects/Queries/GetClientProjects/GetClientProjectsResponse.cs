using FreeLink.Application.UseCase.Projects.DTOs;

namespace FreeLink.Application.UseCase.Projects.Queries.GetClientProjects;

public class GetClientProjectsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ProjectSummaryDto> Projects { get; set; } = new();
}
