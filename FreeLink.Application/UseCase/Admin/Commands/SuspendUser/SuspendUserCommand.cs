using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.SuspendUser;

public class SuspendUserCommand : IRequest<SuspendUserResponse>
{
    public int UserId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int DurationDays { get; set; } // Duración de la suspensión en días
}
