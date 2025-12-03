using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.RemoveSanction;

public class RemoveSanctionCommand : IRequest<RemoveSanctionResponse>
{
    public int SanctionId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
