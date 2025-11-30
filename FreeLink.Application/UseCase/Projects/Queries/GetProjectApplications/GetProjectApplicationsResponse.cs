using FreeLink.Application.UseCase.Projects.DTOs;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectApplications;

public class GetProjectApplicationsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ApplicationDto> Applications { get; set; } = new();
}
