using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetReportDetails;

public class GetReportDetailsQuery : IRequest<GetReportDetailsResponse>
{
    public int ReportId { get; set; }
    public int RequestingAdminId { get; set; }
}
