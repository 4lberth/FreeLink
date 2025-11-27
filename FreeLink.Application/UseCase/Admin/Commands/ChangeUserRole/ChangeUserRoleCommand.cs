using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ChangeUserRole;

public class ChangeUserRoleCommand : IRequest<ChangeUserRoleResponse>
{
    public int UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;
    public int RequestingUserId { get; set; }
}
