namespace FreeLink.Application.UseCase.Projects.Commands.CreateProject;

public class CreateProjectResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
}
