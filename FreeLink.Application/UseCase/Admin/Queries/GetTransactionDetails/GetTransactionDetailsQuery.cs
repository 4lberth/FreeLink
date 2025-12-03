using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetTransactionDetails;

public class GetTransactionDetailsQuery : IRequest<GetTransactionDetailsResponse>
{
    public int TransactionId { get; set; }
    public int RequestingAdminId { get; set; }
}
