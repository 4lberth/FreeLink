using FreeLink.Application.UseCase.Deliverables.DTOs;

namespace FreeLink.Application.UseCase.Deliverables.Queries.GetProjectDeliverables;

public class GetProjectDeliverablesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<DeliverableDto> Deliverables { get; set; } = new();
}
