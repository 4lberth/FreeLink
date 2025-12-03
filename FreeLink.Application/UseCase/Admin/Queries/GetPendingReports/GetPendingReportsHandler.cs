using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingReports;

public class GetPendingReportsHandler : IRequestHandler<GetPendingReportsQuery, GetPendingReportsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingReportsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPendingReportsResponse> Handle(GetPendingReportsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetPendingReportsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver reportes"
                };
            }

            // Obtener reportes pendientes
            var allReports = await _unitOfWork.Repository<Contentreport>().GetAll();
            var pendingReports = allReports.Where(r => r.ReportStatus == "Pendiente");

            pendingReports = pendingReports.OrderBy(r => r.CreatedAt);

            var totalCount = pendingReports.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedReports = pendingReports
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener usuarios y proyectos
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var projects = await _unitOfWork.Repository<Project>().GetAll();
            var usersList = users.ToList();
            var projectsList = projects.ToList();

            // Mapear a DTOs
            var reportDtos = pagedReports.Select(r =>
            {
                var reporter = usersList.FirstOrDefault(u => u.UserId == r.ReporterId);
                var reportedUser = r.ReportedUserId.HasValue ? usersList.FirstOrDefault(u => u.UserId == r.ReportedUserId) : null;
                var project = r.ReportedProjectId.HasValue ? projectsList.FirstOrDefault(p => p.ProjectId == r.ReportedProjectId) : null;

                return new ReportListDto
                {
                    ReportId = r.ReportId,
                    ReporterId = r.ReporterId,
                    ReporterName = reporter?.Email ?? "Usuario no encontrado",
                    ReportedUserId = r.ReportedUserId ?? 0,
                    ReportedUserName = reportedUser?.Email ?? "Usuario no encontrado",
                    ReportType = null,
                    ReportReason = r.ReportReason,
                    ReportStatus = r.ReportStatus,
                    CreatedAt = r.CreatedAt,
                    ProjectId = r.ReportedProjectId,
                    ProjectTitle = project?.Title
                };
            }).ToList();

            return new GetPendingReportsResponse
            {
                Success = true,
                Message = "Reportes obtenidos exitosamente",
                Reports = reportDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetPendingReportsResponse
            {
                Success = false,
                Message = $"Error al obtener reportes: {ex.Message}"
            };
        }
    }
}
