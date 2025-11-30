using FreeLink.Application.UseCase.ActivityLogs.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.ActivityLogs.Queries.GetProjectActivityLog;

public class GetProjectActivityLogHandler : IRequestHandler<GetProjectActivityLogQuery, GetProjectActivityLogResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectActivityLogHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectActivityLogResponse> Handle(GetProjectActivityLogQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el proyecto existe
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectActivityLogResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // Obtener el log de actividades
            var activities = await _unitOfWork.Repository<Projectactivitylog>()
                .GetAsync(a => a.ProjectId == request.ProjectId);

            var activityDtos = new List<ActivityLogDto>();

            foreach (var activity in activities.OrderByDescending(a => a.CreatedAt))
            {
                string? userName = null;
                
                if (activity.UserId.HasValue)
                {
                    var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(activity.UserId.Value);
                    var profile = await _unitOfWork.Repository<Userprofile>()
                        .GetFirstOrDefaultAsync(p => p.UserId == activity.UserId.Value);
                    
                    userName = profile != null 
                        ? $"{profile.FirstName} {profile.LastName}" 
                        : user?.Email;
                }

                activityDtos.Add(new ActivityLogDto
                {
                    ActivityId = activity.ActivityId,
                    ProjectId = activity.ProjectId,
                    UserId = activity.UserId,
                    UserName = userName ?? "Sistema",
                    ActivityType = activity.ActivityType,
                    ActivityDescription = activity.ActivityDescription,
                    CreatedAt = activity.CreatedAt
                });
            }

            return new GetProjectActivityLogResponse
            {
                Success = true,
                Message = $"{activityDtos.Count} actividades encontradas",
                Activities = activityDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProjectActivityLogResponse
            {
                Success = false,
                Message = $"Error al obtener log de actividades: {ex.Message}"
            };
        }
    }
}
