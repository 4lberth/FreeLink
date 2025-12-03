using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ResolveReport;

public class ResolveReportCommand : IRequest<ResolveReportResponse>
{
    public int ReportId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "Eliminar Contenido", "Advertencia al Usuario", "Suspender Usuario", "No Aplica"
}
