using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.UpdateDisputeStatus;

public class UpdateDisputeStatusCommand : IRequest<UpdateDisputeStatusResponse>
{
    public int DisputeId { get; set; }
    public int RequestingAdminId { get; set; }
    public string NewStatus { get; set; } = string.Empty; // "Abierta", "En Revisión", "Resuelta", "Cerrada"
}
