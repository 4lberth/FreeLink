using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Reports.Commands.CreateReport;

public class CreateReportHandler : IRequestHandler<CreateReportCommand, CreateReportResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateReportHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CreateReportResponse> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que se haya especificado al menos un target
            var targetsCount = new[] { request.ReportedUserId, request.ReportedProjectId, request.ReportedMessageId }
                .Count(t => t.HasValue);

            if (targetsCount == 0)
            {
                return new CreateReportResponse
                {
                    Success = false,
                    Message = "Debes especificar qué estás reportando (usuario, proyecto o mensaje)"
                };
            }

            if (targetsCount > 1)
            {
                return new CreateReportResponse
                {
                    Success = false,
                    Message = "Solo puedes reportar una cosa a la vez"
                };
            }

            // 2. Validar que el usuario reportero existe
            var reporter = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.ReporterId);
            if (reporter == null)
            {
                return new CreateReportResponse
                {
                    Success = false,
                    Message = "Usuario reportero no encontrado"
                };
            }

            // 3. Validar que el target existe y que no se está reportando a sí mismo
            if (request.ReportedUserId.HasValue)
            {
                // Validar que el usuario reportado existe
                var reportedUser = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.ReportedUserId.Value);
                if (reportedUser == null)
                {
                    return new CreateReportResponse
                    {
                        Success = false,
                        Message = "El usuario que intentas reportar no existe"
                    };
                }

                // Validar que no se esté reportando a sí mismo
                if (request.ReportedUserId.Value == request.ReporterId)
                {
                    return new CreateReportResponse
                    {
                        Success = false,
                        Message = "No puedes reportarte a ti mismo"
                    };
                }
            }

            if (request.ReportedProjectId.HasValue)
            {
                // Validar que el proyecto existe
                var project = await _unitOfWork.Repository<Project>().GetById(request.ReportedProjectId.Value);
                if (project == null)
                {
                    return new CreateReportResponse
                    {
                        Success = false,
                        Message = "El proyecto que intentas reportar no existe"
                    };
                }
            }

            if (request.ReportedMessageId.HasValue)
            {
                // Validar que el mensaje existe
                var message = await _unitOfWork.Repository<Projectmessage>().GetById(request.ReportedMessageId.Value);
                if (message == null)
                {
                    return new CreateReportResponse
                    {
                        Success = false,
                        Message = "El mensaje que intentas reportar no existe"
                    };
                }
            }

            // 4. Validar que no tenga un reporte pendiente del mismo contenido
            var existingReports = await _unitOfWork.Repository<Contentreport>()
                .GetAsync(r => r.ReporterId == request.ReporterId &&
                              r.ReportStatus == "Pendiente" &&
                              ((request.ReportedUserId.HasValue && r.ReportedUserId == request.ReportedUserId) ||
                               (request.ReportedProjectId.HasValue && r.ReportedProjectId == request.ReportedProjectId) ||
                               (request.ReportedMessageId.HasValue && r.ReportedMessageId == request.ReportedMessageId)));

            if (existingReports.Any())
            {
                return new CreateReportResponse
                {
                    Success = false,
                    Message = "Ya tienes un reporte pendiente sobre este contenido"
                };
            }

            // 5. Crear el reporte
            var report = new Contentreport
            {
                ReporterId = request.ReporterId,
                ReportedUserId = request.ReportedUserId,
                ReportedProjectId = request.ReportedProjectId,
                ReportedMessageId = request.ReportedMessageId,
                ReportReason = request.ReportReason,
                ReportDescription = request.ReportDescription,
                ReportStatus = "Pendiente",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Contentreport>().Add(report);
            await _unitOfWork.Complete();

            // 6. Notificar al usuario que su reporte fue creado
            await _notificationService.CreateNotificationAsync(
                userId: request.ReporterId,
                type: "ReportCreated",
                title: "Reporte Enviado",
                message: "Tu reporte ha sido enviado y será revisado por nuestro equipo. Te notificaremos cuando sea procesado.",
                resourceType: "Report",
                resourceId: report.ReportId
            );

            // 7. Notificar a todos los administradores
            var admins = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetAsync(u => u.UserType == "Administrador");
            var adminIds = admins.Select(a => a.UserId).ToList();

            if (adminIds.Any())
            {
                string reportTarget = request.ReportedUserId.HasValue ? "un usuario"
                    : request.ReportedProjectId.HasValue ? "un proyecto"
                    : "un mensaje";

                await _notificationService.CreateMultipleNotificationsAsync(
                    userIds: adminIds,
                    type: "NewReportPending",
                    title: "Nuevo Reporte Pendiente",
                    message: $"El usuario {reporter.Email} ha reportado {reportTarget}. Razón: {request.ReportReason}",
                    resourceType: "Report",
                    resourceId: report.ReportId
                );
            }

            return new CreateReportResponse
            {
                Success = true,
                Message = "Reporte enviado exitosamente. Nuestro equipo lo revisará pronto.",
                ReportId = report.ReportId
            };
        }
        catch (Exception ex)
        {
            return new CreateReportResponse
            {
                Success = false,
                Message = $"Error al crear reporte: {ex.Message}"
            };
        }
    }
}
