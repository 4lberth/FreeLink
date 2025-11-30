namespace FreeLink.Application.UseCase.Projects.Commands.CreateApplication;

public class CreateApplicationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ApplicationId { get; set; }
}
