using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.IssueWarning;

public class IssueWarningCommand : IRequest<IssueWarningResponse>
{
    public int UserId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
