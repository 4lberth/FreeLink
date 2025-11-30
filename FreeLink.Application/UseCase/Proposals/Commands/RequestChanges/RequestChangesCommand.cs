using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.RequestChanges;

public class RequestChangesCommand : IRequest<RequestChangesResponse>
{
    public int ProposalId { get; set; }
    public int RequestingUserId { get; set; } // Cliente
    public string ChangeReason { get; set; } = string.Empty; // Razón de los cambios solicitados
}
