using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.AssignDispute;

public class AssignDisputeCommand : IRequest<AssignDisputeResponse>
{
    public int DisputeId { get; set; }
    public int MediatorId { get; set; } // Admin to assign as mediator
    public int RequestingAdminId { get; set; }
}
