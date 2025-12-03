using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ResolveDispute;

public class ResolveDisputeCommand : IRequest<ResolveDisputeResponse>
{
    public int DisputeId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public int? WinningPartyId { get; set; } // Optional: which party won the dispute
}
