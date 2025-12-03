using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.BanUser;

public class BanUserCommand : IRequest<BanUserResponse>
{
    public int UserId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
