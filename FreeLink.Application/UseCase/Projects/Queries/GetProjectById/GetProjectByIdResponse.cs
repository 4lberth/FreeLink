using FreeLink.Application.UseCase.Projects.DTOs;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectById;

public class GetProjectByIdResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ProjectDto? Project { get; set; }
}
