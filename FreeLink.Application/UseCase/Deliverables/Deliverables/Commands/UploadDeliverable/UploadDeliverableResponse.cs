namespace FreeLink.Application.UseCase.Deliverables.Commands.UploadDeliverable;

public class UploadDeliverableResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? DeliverableId { get; set; }
}
