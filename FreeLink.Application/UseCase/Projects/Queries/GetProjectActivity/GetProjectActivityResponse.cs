using FreeLink.Application.UseCase.Projects.DTOs;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectActivity;

public class GetProjectActivityResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<ActivitySummaryDto> Activities { get; set; } = new();
}
