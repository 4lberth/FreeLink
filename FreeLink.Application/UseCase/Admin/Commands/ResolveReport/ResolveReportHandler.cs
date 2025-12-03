using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ResolveReport;

public class ResolveReportHandler : IRequestHandler<ResolveReportCommand, ResolveReportResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public ResolveReportHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<ResolveReportResponse> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new ResolveReportResponse
                {
                    Success = false,
                    Message = "No tienes permisos para resolver reportes"
                };
            }

            // Obtener el reporte
            var report = await _unitOfWork.Repository<Contentreport>().GetById(request.ReportId);
            if (report == null)
            {
                return new ResolveReportResponse
                {
                    Success = false,
                    Message = "Reporte no encontrado"
                };
            }

            // Validar que el reporte está pendiente
            if (report.ReportStatus != "Pendiente")
            {
                return new ResolveReportResponse
                {
                    Success = false,
                    Message = "El reporte ya ha sido procesado"
                };
            }

            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(request.Resolution) || string.IsNullOrWhiteSpace(request.Action))
            {
                return new ResolveReportResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una resolución y acción"
                };
            }

            // Actualizar el reporte
            report.ReportStatus = "Resuelto";
            report.ReviewedBy = request.RequestingAdminId;
            report.ReviewedAt = DateTime.UtcNow;
            report.Resolution = request.Resolution;

            await _unitOfWork.Repository<Contentreport>().Update(report);

            // Notificar al usuario que reportó
            await _notificationService.CreateNotificationAsync(
                report.ReporterId,
                "admin_action",
                "Reporte Resuelto",
                $"Tu reporte ha sido revisado y resuelto. Acción tomada: {request.Action}",
                "report",
                report.ReportId
            );

            // Notificar al usuario reportado
            if (report.ReportedUserId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    report.ReportedUserId.Value,
                    "admin_action",
                    "Reporte sobre tu contenido",
                    $"Se ha resuelto un reporte sobre tu contenido. Acción: {request.Action}",
                    "report",
                    report.ReportId
                );
            }

            // Registrar actividad del administrador
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "ResolveReport",
                $"Resolvió reporte #{request.ReportId} con acción: {request.Action}"
            );

            await _unitOfWork.Complete();

            return new ResolveReportResponse
            {
                Success = true,
                Message = "Reporte resuelto exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ResolveReportResponse
            {
                Success = false,
                Message = $"Error al resolver reporte: {ex.Message}"
            };
        }
    }
}
