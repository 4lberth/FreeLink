using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectActivity;

public class GetProjectActivityHandler : IRequestHandler<GetProjectActivityQuery, GetProjectActivityResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectActivityHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectActivityResponse> Handle(GetProjectActivityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectActivityResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Verificar permisos
            if (project.ClientId != request.RequestingUserId && project.AssignedFreelancerId != request.RequestingUserId)
            {
                return new GetProjectActivityResponse
                {
                    Success = false,
                    Message = "No tienes permiso para ver la actividad de este proyecto"
                };
            }

            // 3. Obtener actividades
            var activityQuery = await _unitOfWork.Repository<Projectactivitylog>()
                .GetAsync(a => a.ProjectId == request.ProjectId);
            
            var activities = activityQuery
                .OrderByDescending(a => a.CreatedAt)
                .Take(request.Limit ?? 20)
                .ToList();

            // 4. Obtener usuarios
            var userIds = activities.Where(a => a.UserId.HasValue).Select(a => a.UserId!.Value).Distinct().ToList();
            var usersQuery = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetAsync(u => userIds.Contains(u.UserId));
            var users = usersQuery.ToList();

            // 5. Mapear a DTOs
            var activityDtos = activities.Select(a =>
            {
                var user = a.UserId.HasValue
                    ? users.FirstOrDefault(u => u.UserId == a.UserId.Value)
                    : null;

                return new ActivitySummaryDto
                {
                    ActivityId = a.ActivityId,
                    UserName = user?.Email ?? "Sistema",
                    ActivityType = a.ActivityType,
                    Description = a.ActivityDescription ?? "",
                    CreatedAt = a.CreatedAt
                };
            }).ToList();

            return new GetProjectActivityResponse
            {
                Success = true,
                Message = "Actividad obtenida exitosamente",
                Activities = activityDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProjectActivityResponse
            {
                Success = false,
                Message = $"Error al obtener actividad: {ex.Message}"
            };
        }
    }
}
