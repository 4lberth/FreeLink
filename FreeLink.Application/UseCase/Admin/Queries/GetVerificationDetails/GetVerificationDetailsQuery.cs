using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetVerificationDetails;

public class GetVerificationDetailsQuery : IRequest<GetVerificationDetailsResponse>
{
    public int VerificationId { get; set; }
    public int RequestingAdminId { get; set; }
}
