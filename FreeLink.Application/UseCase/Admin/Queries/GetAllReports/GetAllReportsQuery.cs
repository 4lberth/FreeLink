using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllReports;

public class GetAllReportsQuery : IRequest<GetAllReportsResponse>
{
    public int RequestingAdminId { get; set; }
    public string? StatusFilter { get; set; } // "Pendiente", "En Revisión", "Resuelto", "Rechazado"
    public string? ReportType { get; set; } // "Usuario", "Proyecto", "Mensaje"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
