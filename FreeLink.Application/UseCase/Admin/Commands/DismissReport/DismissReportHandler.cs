using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.DismissReport;

public class DismissReportHandler : IRequestHandler<DismissReportCommand, DismissReportResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public DismissReportHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<DismissReportResponse> Handle(DismissReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new DismissReportResponse
                {
                    Success = false,
                    Message = "No tienes permisos para desestimar reportes"
                };
            }

            // Obtener el reporte
            var report = await _unitOfWork.Repository<Contentreport>().GetById(request.ReportId);
            if (report == null)
            {
                return new DismissReportResponse
                {
                    Success = false,
                    Message = "Reporte no encontrado"
                };
            }

            // Validar que el reporte está pendiente
            if (report.ReportStatus != "Pendiente")
            {
                return new DismissReportResponse
                {
                    Success = false,
                    Message = "El reporte ya ha sido procesado"
                };
            }

            // Validar razón
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return new DismissReportResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una razón para desestimar el reporte"
                };
            }

            // Actualizar el reporte
            report.ReportStatus = "Desestimado";
            report.ReviewedBy = request.RequestingAdminId;
            report.ReviewedAt = DateTime.UtcNow;
            report.Resolution = "No Aplica";

            await _unitOfWork.Repository<Contentreport>().Update(report);

            // Notificar al usuario que reportó
            await _notificationService.CreateNotificationAsync(
                report.ReporterId,
                "admin_action",
                "Reporte Desestimado",
                $"Tu reporte ha sido revisado y desestimado. Razón: {request.Reason}",
                "report",
                report.ReportId
            );

            // Registrar actividad del administrador
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "DismissReport",
                $"Desestimó reporte #{request.ReportId}"
            );

            await _unitOfWork.Complete();

            return new DismissReportResponse
            {
                Success = true,
                Message = "Reporte desestimado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DismissReportResponse
            {
                Success = false,
                Message = $"Error al desestimar reporte: {ex.Message}"
            };
        }
    }
}
