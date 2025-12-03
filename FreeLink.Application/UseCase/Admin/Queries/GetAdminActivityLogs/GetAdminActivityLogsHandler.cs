using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAdminActivityLogs;

public class GetAdminActivityLogsHandler : IRequestHandler<GetAdminActivityLogsQuery, GetAdminActivityLogsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminActivityLogsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAdminActivityLogsResponse> Handle(GetAdminActivityLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetAdminActivityLogsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver logs de actividad"
                };
            }

            // Obtener todos los logs
            var allLogs = await _unitOfWork.Repository<Adminactivitylog>().GetAll();
            var logs = allLogs.AsEnumerable();

            // Aplicar filtros
            if (request.AdminId.HasValue)
            {
                logs = logs.Where(l => l.AdminId == request.AdminId.Value);
            }

            if (!string.IsNullOrEmpty(request.ActionType))
            {
                logs = logs.Where(l => l.ActionType == request.ActionType);
            }

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

            // Obtener usuarios
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear a DTOs
            var logDtos = pagedLogs.Select(l =>
            {
                var adminUser = usersList.FirstOrDefault(u => u.UserId == l.AdminId);
                return new AdminActivityLogDto
                {
                    LogId = l.LogId,
                    AdminId = l.AdminId,
                    AdminName = adminUser?.Email ?? "Administrador no encontrado",
                    ActionType = l.ActionType,
                    ActionDetails = l.ActionDescription,
                    CreatedAt = l.CreatedAt,
                    IpAddress = l.IpAddress
                };
            }).ToList();

            return new GetAdminActivityLogsResponse
            {
                Success = true,
                Message = "Logs obtenidos exitosamente",
                Logs = logDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetAdminActivityLogsResponse
            {
                Success = false,
                Message = $"Error al obtener logs: {ex.Message}"
            };
        }
    }
}
