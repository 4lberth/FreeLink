using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingDisputes;

public class GetPendingDisputesHandler : IRequestHandler<GetPendingDisputesQuery, GetPendingDisputesResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingDisputesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPendingDisputesResponse> Handle(GetPendingDisputesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetPendingDisputesResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver disputas"
                };
            }

            // Obtener todas las disputas
            var allDisputes = await _unitOfWork.Repository<Dispute>().GetAll();
            var disputes = allDisputes.AsEnumerable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Status))
            {
                disputes = disputes.Where(d => d.DisputeStatus == request.Status);
            }
            else
            {
                // Por defecto, solo disputas abiertas o en revisión
                disputes = disputes.Where(d => d.DisputeStatus == "Abierta" || d.DisputeStatus == "En Revisión");
            }

            disputes = disputes.OrderByDescending(d => d.CreatedAt);

            var totalCount = disputes.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedDisputes = disputes
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener datos relacionados
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var projects = await _unitOfWork.Repository<Project>().GetAll();
            var usersList = users.ToList();
            var projectsList = projects.ToList();

            // Mapear a DTOs
            var disputeDtos = pagedDisputes.Select(d =>
            {
                var initiator = usersList.FirstOrDefault(u => u.UserId == d.InitiatorId);
                var respondent = usersList.FirstOrDefault(u => u.UserId == d.RespondentId);
                var mediator = d.MediatorId.HasValue ? usersList.FirstOrDefault(u => u.UserId == d.MediatorId.Value) : null;
                var project = projectsList.FirstOrDefault(p => p.ProjectId == d.ProjectId);

                return new DisputeListDto
                {
                    DisputeId = d.DisputeId,
                    ProjectId = d.ProjectId,
                    ProjectTitle = project?.Title,
                    InitiatorId = d.InitiatorId,
                    InitiatorName = initiator?.Email ?? "Usuario no encontrado",
                    RespondentId = d.RespondentId,
                    RespondentName = respondent?.Email ?? "Usuario no encontrado",
                    DisputeReason = d.DisputeReason,
                    DisputeStatus = d.DisputeStatus,
                    CreatedAt = d.CreatedAt,
                    MediatorId = d.MediatorId,
                    MediatorName = mediator?.Email,
                    ResolvedAt = d.ResolvedAt
                };
            }).ToList();

            return new GetPendingDisputesResponse
            {
                Success = true,
                Message = "Disputas obtenidas exitosamente",
                Disputes = disputeDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetPendingDisputesResponse
            {
                Success = false,
                Message = $"Error al obtener disputas: {ex.Message}"
            };
        }
    }
}
