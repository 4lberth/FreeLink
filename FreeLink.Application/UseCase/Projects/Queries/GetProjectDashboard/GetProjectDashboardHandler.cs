using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectDashboard;

public class GetProjectDashboardHandler : IRequestHandler<GetProjectDashboardQuery, GetProjectDashboardResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectDashboardHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectDashboardResponse> Handle(GetProjectDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectDashboardResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Verificar permisos
            if (project.ClientId != request.RequestingUserId && project.AssignedFreelancerId != request.RequestingUserId)
            {
                return new GetProjectDashboardResponse
                {
                    Success = false,
                    Message = "No tienes permiso para ver este dashboard"
                };
            }

            // 3. Obtener datos del proyecto
            var clientQuery = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetAsync(u => u.UserId == project.ClientId);
            var client = clientQuery.FirstOrDefault();

            // 4. Obtener entregables
            var deliverablesQuery = await _unitOfWork.Repository<Projectdeliverable>()
                .GetAsync(d => d.ProjectId == request.ProjectId);
            var deliverables = deliverablesQuery.ToList();

            // 5. Calcular progreso
            var totalDeliverables = deliverables.Count;
            var aprobados = deliverables.Count(d => d.DeliverableStatus == "Aprobado");
            var rechazados = deliverables.Count(d => d.DeliverableStatus == "Rechazado");
            var enRevision = deliverables.Count(d => d.DeliverableStatus == "En Revisión");
            var pendientes = deliverables.Count(d => d.DeliverableStatus == "Pendiente");

            var percentageComplete = totalDeliverables > 0
                ? Math.Round((decimal)aprobados / totalDeliverables * 100, 2)
                : 0;

            // 6. Obtener mensajes
            var messagesQuery = await _unitOfWork.Repository<Projectmessage>()
                .GetAsync(m => m.ProjectId == request.ProjectId);
            var messages = messagesQuery.OrderByDescending(m => m.SentAt).ToList();

            var unreadMessages = messages.Count(m => m.SenderId != request.RequestingUserId && !(m.IsRead ?? false));

            // 7. Obtener actividades recientes
            var activityQuery = await _unitOfWork.Repository<Projectactivitylog>()
                .GetAsync(a => a.ProjectId == request.ProjectId);
            var activities = activityQuery.OrderByDescending(a => a.CreatedAt).Take(10).ToList();

            // 8. Obtener usuarios para actividades
            var activityUserIds = activities.Where(a => a.UserId.HasValue).Select(a => a.UserId!.Value).Distinct().ToList();
            var activityUsersQuery = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetAsync(u => activityUserIds.Contains(u.UserId));
            var activityUsers = activityUsersQuery.ToList();

            // 9. Obtener usuarios para mensajes
            var messageUserIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var messageUsersQuery = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetAsync(u => messageUserIds.Contains(u.UserId));
            var messageUsers = messageUsersQuery.ToList();

            // 10. Mapear a DTOs
            var dashboard = new ProjectDashboardDto
            {
                Project = new ProjectSummaryDto
                {
                    ProjectId = project.ProjectId,
                    Title = project.Title,
                    Description = project.Description,
                    Budget = project.Budget,
                    DeadlineDate = project.DeadlineDate,
                    ProjectStatus = project.ProjectStatus ?? "Publicado",
                    ClientName = client?.Email ?? "Cliente",
                    RequiredSkills = new List<SkillDto>()
                },
                Progress = new ProjectProgressDto
                {
                    TotalDeliverables = totalDeliverables,
                    DeliverablesAprobados = aprobados,
                    DeliverablesRechazados = rechazados,
                    DeliverablesEnRevision = enRevision,
                    DeliverablesPendientes = pendientes,
                    PercentageComplete = percentageComplete,
                    TotalMessages = messages.Count,
                    UnreadMessages = unreadMessages
                },
                RecentDeliverables = deliverables
                    .OrderByDescending(d => d.SubmittedAt ?? DateTime.MinValue)
                    .Take(5)
                    .Select(d => new DeliverableSummaryDto
                    {
                        DeliverableId = d.DeliverableId,
                        Title = d.Title,
                        Status = d.DeliverableStatus ?? "Pendiente",
                        SubmittedAt = d.SubmittedAt,
                        ReviewedAt = d.ReviewedAt
                    }).ToList(),
                RecentMessages = messages
                    .Take(5)
                    .Select(m =>
                    {
                        var sender = messageUsers.FirstOrDefault(u => u.UserId == m.SenderId);
                        var preview = m.MessageText.Length > 50
                            ? m.MessageText.Substring(0, 50) + "..."
                            : m.MessageText;

                        return new MessageSummaryDto
                        {
                            MessageId = m.MessageId,
                            SenderName = sender?.Email ?? "Usuario",
                            ContentPreview = preview,
                            SentAt = m.SentAt,
                            IsRead = m.IsRead ?? false
                        };
                    }).ToList(),
                RecentActivity = activities.Select(a =>
                {
                    var user = a.UserId.HasValue
                        ? activityUsers.FirstOrDefault(u => u.UserId == a.UserId.Value)
                        : null;

                    return new ActivitySummaryDto
                    {
                        ActivityId = a.ActivityId,
                        UserName = user?.Email ?? "Sistema",
                        ActivityType = a.ActivityType,
                        Description = a.ActivityDescription ?? "",
                        CreatedAt = a.CreatedAt
                    };
                }).ToList()
            };

            return new GetProjectDashboardResponse
            {
                Success = true,
                Message = "Dashboard obtenido exitosamente",
                Dashboard = dashboard
            };
        }
        catch (Exception ex)
        {
            return new GetProjectDashboardResponse
            {
                Success = false,
                Message = $"Error al obtener dashboard: {ex.Message}"
            };
        }
    }
}
