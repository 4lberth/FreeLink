using FreeLink.Application.UseCase.Deliverables.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Queries.GetProjectDeliverables;

public class GetProjectDeliverablesHandler : IRequestHandler<GetProjectDeliverablesQuery, GetProjectDeliverablesResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectDeliverablesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectDeliverablesResponse> Handle(GetProjectDeliverablesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectDeliverablesResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Verificar permisos
            if (project.ClientId != request.RequestingUserId && project.AssignedFreelancerId != request.RequestingUserId)
            {
                return new GetProjectDeliverablesResponse
                {
                    Success = false,
                    Message = "No tienes permiso para ver los entregables de este proyecto"
                };
            }

            // 3. Obtener entregables
            var deliverablesQuery = await _unitOfWork.Repository<Projectdeliverable>()
                .GetAsync(d => d.ProjectId == request.ProjectId);
            
            var deliverables = deliverablesQuery.OrderByDescending(d => d.SubmittedAt ?? DateTime.MinValue).ToList();

            // 4. Filtrar según el rol: Cliente NO ve entregables "Pendientes"
            if (request.RequestingUserId == project.ClientId)
            {
                deliverables = deliverables
                    .Where(d => d.DeliverableStatus != "Pendiente")
                    .ToList();
            }
            // Freelancer ve todos sus entregables (incluyendo Pendientes)

            // 5. Obtener archivos de todos los entregables
            var deliverableIds = deliverables.Select(d => d.DeliverableId).ToList();
            var filesQuery = await _unitOfWork.Repository<Deliverablefile>()
                .GetAsync(f => deliverableIds.Contains(f.DeliverableId));
            var files = filesQuery.ToList();

            // 6. Mapear a DTOs
            var deliverableDtos = deliverables.Select(d =>
            {
                var deliverableFiles = files.Where(f => f.DeliverableId == d.DeliverableId).ToList();

                return new DeliverableDto
                {
                    DeliverableId = d.DeliverableId,
                    ProjectId = d.ProjectId,
                    Title = d.Title,
                    Description = d.Description,
                    Status = d.DeliverableStatus ?? "Pendiente",
                    UploadedAt = d.SubmittedAt ?? DateTime.UtcNow,
                    SubmittedAt = d.SubmittedAt,
                    ReviewedAt = d.ReviewedAt,
                    ReviewComments = d.ReviewComments,
                    Files = deliverableFiles.Select(f => new DeliverableFileDto
                    {
                        FileId = f.FileId,
                        FileName = f.FileName,
                        FileUrl = f.FileUrl,
                        FileSize = f.FileSize ?? 0,
                        UploadedAt = f.UploadedAt
                    }).ToList()
                };
            }).ToList();

            return new GetProjectDeliverablesResponse
            {
                Success = true,
                Message = "Entregables obtenidos exitosamente",
                Deliverables = deliverableDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProjectDeliverablesResponse
            {
                Success = false,
                Message = $"Error al obtener entregables: {ex.Message}"
            };
        }
    }
}
