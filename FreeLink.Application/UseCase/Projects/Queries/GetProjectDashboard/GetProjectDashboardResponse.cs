using FreeLink.Application.UseCase.Projects.DTOs;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectDashboard;

public class GetProjectDashboardResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public ProjectDashboardDto? Dashboard { get; set; }
}
