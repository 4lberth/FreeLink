using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetTransactionStatistics;

public class GetTransactionStatisticsQuery : IRequest<GetTransactionStatisticsResponse>
{
    public int RequestingAdminId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
