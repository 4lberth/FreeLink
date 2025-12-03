using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetReportDetails;

public class GetReportDetailsHandler : IRequestHandler<GetReportDetailsQuery, GetReportDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReportDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetReportDetailsResponse> Handle(GetReportDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetReportDetailsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver detalles de reportes"
                };
            }

            // Obtener el reporte
            var report = await _unitOfWork.Repository<Contentreport>().GetById(request.ReportId);
            if (report == null)
            {
                return new GetReportDetailsResponse
                {
                    Success = false,
                    Message = "Reporte no encontrado"
                };
            }

            // Obtener usuarios relacionados
            var reporter = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(report.ReporterId);
            var reportedUser = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(report.ReportedUserId);

            FreeLink.Domain.Entities.User? reviewer = null;
            if (report.ReviewedBy.HasValue)
            {
                reviewer = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(report.ReviewedBy.Value);
            }

            Project? project = null;
            if (report.ReportedProjectId.HasValue)
            {
                project = await _unitOfWork.Repository<Project>().GetById(report.ReportedProjectId.Value);
            }

            var reportDetails = new ReportDetailsDto
            {
                ReportId = report.ReportId,
                ReporterId = report.ReporterId,
                ReporterName = reporter?.Email ?? "Usuario no encontrado",
                ReporterEmail = reporter?.Email ?? "",
                ReportedUserId = report.ReportedUserId ?? 0,
                ReportedUserName = reportedUser?.Email ?? "Usuario no encontrado",
                ReportedUserEmail = reportedUser?.Email ?? "",
                ReportType = null,
                ReportReason = report.ReportReason,
                Description = report.ReportDescription,
                EvidenceUrl = null,
                ReportStatus = report.ReportStatus,
                CreatedAt = report.CreatedAt,
                ReviewedAt = report.ReviewedAt,
                ReviewedBy = report.ReviewedBy,
                ReviewedByName = reviewer?.Email,
                Resolution = report.Resolution,
                AdminAction = report.Resolution,
                ProjectId = report.ReportedProjectId,
                ProjectTitle = project?.Title
            };

            return new GetReportDetailsResponse
            {
                Success = true,
                Message = "Detalles de reporte obtenidos exitosamente",
                Data = reportDetails
            };
        }
        catch (Exception ex)
        {
            return new GetReportDetailsResponse
            {
                Success = false,
                Message = $"Error al obtener detalles del reporte: {ex.Message}"
            };
        }
    }
}
