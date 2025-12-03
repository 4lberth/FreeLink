using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllReports;

public class GetAllReportsHandler : IRequestHandler<GetAllReportsQuery, GetAllReportsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllReportsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllReportsResponse> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetAllReportsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver reportes"
                };
            }

            // Obtener todos los reportes
            var allReportsQuery = await _unitOfWork.Repository<Contentreport>().GetAll();
            var allReports = allReportsQuery.ToList();

            // Filtrar por estado si se especificó
            if (!string.IsNullOrWhiteSpace(request.StatusFilter))
            {
                allReports = allReports.Where(r => r.ReportStatus == request.StatusFilter).ToList();
            }

            // Filtrar por tipo si se especificó
            if (!string.IsNullOrWhiteSpace(request.ReportType))
            {
                allReports = request.ReportType switch
                {
                    "Usuario" => allReports.Where(r => r.ReportedUserId.HasValue).ToList(),
                    "Proyecto" => allReports.Where(r => r.ReportedProjectId.HasValue).ToList(),
                    "Mensaje" => allReports.Where(r => r.ReportedMessageId.HasValue).ToList(),
                    _ => allReports
                };
            }

            // Ordenar por más recientes primero
            allReports = allReports.OrderByDescending(r => r.CreatedAt).ToList();

            // Paginación
            var totalCount = allReports.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
            var pagedReports = allReports
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener información de usuarios y proyectos
            var userIds = pagedReports.Select(r => r.ReporterId)
                .Union(pagedReports.Where(r => r.ReportedUserId.HasValue).Select(r => r.ReportedUserId!.Value))
                .Union(pagedReports.Where(r => r.ReviewedBy.HasValue).Select(r => r.ReviewedBy!.Value))
                .Distinct()
                .ToList();

            var usersQuery = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetAsync(u => userIds.Contains(u.UserId));
            var users = usersQuery.ToList();

            var projectIds = pagedReports
                .Where(r => r.ReportedProjectId.HasValue)
                .Select(r => r.ReportedProjectId!.Value)
                .Distinct()
                .ToList();

            var projectsQuery = projectIds.Any()
                ? await _unitOfWork.Repository<Project>().GetAsync(p => projectIds.Contains(p.ProjectId))
                : Enumerable.Empty<Project>();
            var projects = projectsQuery.ToList();

            // Mapear a DTOs
            var reportDtos = pagedReports.Select(r =>
            {
                var reporter = users.FirstOrDefault(u => u.UserId == r.ReporterId);
                var reportedUser = r.ReportedUserId.HasValue
                    ? users.FirstOrDefault(u => u.UserId == r.ReportedUserId.Value)
                    : null;
                var project = r.ReportedProjectId.HasValue
                    ? projects.FirstOrDefault(p => p.ProjectId == r.ReportedProjectId.Value)
                    : null;
                var reviewer = r.ReviewedBy.HasValue
                    ? users.FirstOrDefault(u => u.UserId == r.ReviewedBy.Value)
                    : null;

                string reportType = r.ReportedUserId.HasValue ? "Usuario"
                    : r.ReportedProjectId.HasValue ? "Proyecto"
                    : r.ReportedMessageId.HasValue ? "Mensaje"
                    : "Desconocido";

                return new ReportDto
                {
                    ReportId = r.ReportId,
                    ReporterId = r.ReporterId,
                    ReporterEmail = reporter?.Email ?? "Usuario desconocido",
                    ReportedUserId = r.ReportedUserId,
                    ReportedUserEmail = reportedUser?.Email,
                    ReportedProjectId = r.ReportedProjectId,
                    ReportedProjectTitle = project?.Title,
                    ReportedMessageId = r.ReportedMessageId,
                    ReportReason = r.ReportReason,
                    ReportDescription = r.ReportDescription,
                    ReportStatus = r.ReportStatus ?? "Pendiente",
                    CreatedAt = r.CreatedAt,
                    ReviewedAt = r.ReviewedAt,
                    ReviewedBy = r.ReviewedBy,
                    ReviewedByEmail = reviewer?.Email,
                    Resolution = r.Resolution,
                    ReportType = reportType
                };
            }).ToList();

            return new GetAllReportsResponse
            {
                Success = true,
                Message = $"Se encontraron {totalCount} reportes",
                Reports = reportDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetAllReportsResponse
            {
                Success = false,
                Message = $"Error al obtener reportes: {ex.Message}"
            };
        }
    }
}
