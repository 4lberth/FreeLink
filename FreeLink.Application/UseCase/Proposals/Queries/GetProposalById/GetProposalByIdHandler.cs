using FreeLink.Application.UseCase.Proposals.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalById;

public class GetProposalByIdHandler : IRequestHandler<GetProposalByIdQuery, GetProposalByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProposalByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProposalByIdResponse> Handle(GetProposalByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la propuesta
            var proposal = await _unitOfWork.Repository<Proposal>().GetById(request.ProposalId);
            if (proposal == null)
            {
                return new GetProposalByIdResponse
                {
                    Success = false,
                    Message = "Propuesta no encontrada"
                };
            }

            // 2. Obtener el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(proposal.ProjectId);
            if (project == null)
            {
                return new GetProposalByIdResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Verificar permisos (solo cliente dueño o freelancer dueño pueden ver)
            if (request.RequestingUserId.HasValue)
            {
                bool isClient = project.ClientId == request.RequestingUserId.Value;
                bool isFreelancer = proposal.FreelancerId == request.RequestingUserId.Value;

                if (!isClient && !isFreelancer)
                {
                    return new GetProposalByIdResponse
                    {
                        Success = false,
                        Message = "No tienes permiso para ver esta propuesta"
                    };
                }
            }

            // 4. Obtener el freelancer
            var freelancer = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(proposal.FreelancerId);
            var freelancerProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(p => p.UserId == proposal.FreelancerId);

            string freelancerName = freelancerProfile != null
                ? $"{freelancerProfile.FirstName} {freelancerProfile.LastName}"
                : freelancer?.Email ?? "Desconocido";

            // 5. Obtener desglose de costos
            var costBreakdown = await _unitOfWork.Repository<Proposalcostbreakdown>()
                .GetAsync(c => c.ProposalId == request.ProposalId);

            var costBreakdownDtos = costBreakdown
                .OrderBy(c => c.ItemOrder)
                .Select(c => new CostBreakdownItemDto
                {
                    ItemDescription = c.ItemDescription,
                    Amount = c.Amount,
                    ItemOrder = c.ItemOrder ?? 0
                })
                .ToList();

            // 6. Obtener cronograma
            var timeline = await _unitOfWork.Repository<Proposaltimeline>()
                .GetAsync(t => t.ProposalId == request.ProposalId);

            var timelineDtos = timeline
                .OrderBy(t => t.ItemOrder)
                .Select(t => new TimelineMilestoneDto
                {
                    MilestoneName = t.MilestoneName,
                    Description = t.Description,
                    EstimatedDuration = t.EstimatedDuration ?? 0,
                    ItemOrder = t.ItemOrder ?? 0
                })
                .ToList();

            // 7. Obtener entregables
            var deliverables = await _unitOfWork.Repository<Proposaldeliverable>()
                .GetAsync(d => d.ProposalId == request.ProposalId);

            var deliverableDtos = deliverables
                .OrderBy(d => d.ItemOrder)
                .Select(d => new DeliverableItemDto
                {
                    DeliverableName = d.DeliverableName,
                    Description = d.Description,
                    ItemOrder = d.ItemOrder ?? 0
                })
                .ToList();

            // 8. Construir el DTO completo
            var proposalDto = new ProposalDto
            {
                ProposalId = proposal.ProposalId,
                ProjectId = proposal.ProjectId,
                ProjectTitle = project.Title,
                FreelancerId = proposal.FreelancerId,
                FreelancerName = freelancerName,
                VersionNumber = proposal.VersionNumber ?? 1,
                TotalCost = proposal.TotalCost,
                ProposalStatus = proposal.ProposalStatus ?? "Pendiente",
                CreatedAt = proposal.CreatedAt,
                UpdatedAt = proposal.UpdatedAt,
                CostBreakdown = costBreakdownDtos,
                Timeline = timelineDtos,
                Deliverables = deliverableDtos
            };

            return new GetProposalByIdResponse
            {
                Success = true,
                Message = "Propuesta encontrada",
                Proposal = proposalDto
            };
        }
        catch (Exception ex)
        {
            return new GetProposalByIdResponse
            {
                Success = false,
                Message = $"Error al obtener propuesta: {ex.Message}"
            };
        }
    }
}
