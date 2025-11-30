using FreeLink.Application.UseCase.Proposals.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalsByProject;

public class GetProposalsByProjectHandler : IRequestHandler<GetProposalsByProjectQuery, GetProposalsByProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProposalsByProjectHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProposalsByProjectResponse> Handle(GetProposalsByProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el proyecto existe
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProposalsByProjectResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Verificar permisos (solo cliente dueño o freelancer asignado pueden ver)
            if (request.RequestingUserId.HasValue)
            {
                bool isClient = project.ClientId == request.RequestingUserId.Value;
                bool isFreelancer = project.AssignedFreelancerId == request.RequestingUserId.Value;

                if (!isClient && !isFreelancer)
                {
                    return new GetProposalsByProjectResponse
                    {
                        Success = false,
                        Message = "No tienes permiso para ver las propuestas de este proyecto"
                    };
                }
            }

            // 3. Obtener todas las propuestas del proyecto
            var proposals = await _unitOfWork.Repository<Proposal>()
                .GetAsync(p => p.ProjectId == request.ProjectId);

            var proposalsList = proposals.OrderByDescending(p => p.VersionNumber).ToList();

            // 4. Construir DTOs resumidos
            var proposalDtos = new List<ProposalSummaryDto>();

            foreach (var proposal in proposalsList)
            {
                // Obtener freelancer
                var freelancer = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(proposal.FreelancerId);
                var freelancerProfile = await _unitOfWork.Repository<Userprofile>()
                    .GetFirstOrDefaultAsync(p => p.UserId == proposal.FreelancerId);

                string freelancerName = freelancerProfile != null
                    ? $"{freelancerProfile.FirstName} {freelancerProfile.LastName}"
                    : freelancer?.Email ?? "Desconocido";

                proposalDtos.Add(new ProposalSummaryDto
                {
                    ProposalId = proposal.ProposalId,
                    ProjectId = proposal.ProjectId,
                    ProjectTitle = project.Title,
                    FreelancerId = proposal.FreelancerId,
                    FreelancerName = freelancerName,
                    VersionNumber = proposal.VersionNumber ?? 1,
                    TotalCost = proposal.TotalCost,
                    ProposalStatus = string.IsNullOrWhiteSpace(proposal.ProposalStatus) ? "Enviada" : proposal.ProposalStatus,
                    CreatedAt = proposal.CreatedAt,
                    UpdatedAt = proposal.UpdatedAt
                });
            }

            return new GetProposalsByProjectResponse
            {
                Success = true,
                Message = $"{proposalDtos.Count} propuesta(s) encontrada(s)",
                Proposals = proposalDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProposalsByProjectResponse
            {
                Success = false,
                Message = $"Error al obtener propuestas: {ex.Message}"
            };
        }
    }
}
