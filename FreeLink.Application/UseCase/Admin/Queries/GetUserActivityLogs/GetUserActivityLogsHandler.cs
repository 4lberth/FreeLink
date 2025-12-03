using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetUserActivityLogs;

public class GetUserActivityLogsHandler : IRequestHandler<GetUserActivityLogsQuery, GetUserActivityLogsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserActivityLogsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserActivityLogsResponse> Handle(GetUserActivityLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetUserActivityLogsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver logs de actividad de usuarios"
                };
            }

            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new GetUserActivityLogsResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener logs del usuario desde ProjectActivityLog
            var allLogs = await _unitOfWork.Repository<Projectactivitylog>().GetAll();
            var logs = allLogs.Where(l => l.UserId == request.UserId).AsEnumerable();

            // Aplicar filtros de fecha
            if (request.StartDate.HasValue)
            {
                logs = logs.Where(l => l.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                logs = logs.Where(l => l.CreatedAt <= request.EndDate.Value);
            }

            logs = logs.OrderByDescending(l => l.CreatedAt);

            var totalCount = logs.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedLogs = logs
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Mapear a DTOs
            var logDtos = pagedLogs.Select(l => new UserActivityLogDto
            {
                LogId = l.ActivityId,
                ActivityType = l.ActivityType,
                Description = l.ActivityDescription,
                CreatedAt = l.CreatedAt,
                RelatedEntity = "Project",
                RelatedEntityId = l.ProjectId
            }).ToList();

            return new GetUserActivityLogsResponse
            {
                Success = true,
                Message = "Logs de usuario obtenidos exitosamente",
                Logs = logDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetUserActivityLogsResponse
            {
                Success = false,
                Message = $"Error al obtener logs de usuario: {ex.Message}"
            };
        }
    }
}
