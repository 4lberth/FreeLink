using FreeLink.Application.UseCase.Contracts.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Contracts.Queries.GetContractByProject;

public class GetContractByProjectHandler : IRequestHandler<GetContractByProjectQuery, GetContractByProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetContractByProjectHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetContractByProjectResponse> Handle(GetContractByProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetContractByProjectResponse { Success = false, Message = "Proyecto no encontrado" };
            }

            // Check permissions
            if (request.RequestingUserId.HasValue)
            {
                bool isClient = project.ClientId == request.RequestingUserId.Value;
                bool isFreelancer = project.AssignedFreelancerId == request.RequestingUserId.Value;

                if (!isClient && !isFreelancer)
                {
                    return new GetContractByProjectResponse { Success = false, Message = "No tienes permiso para ver este contrato" };
                }
            }

            var contract = await _unitOfWork.Repository<Contract>()
                .GetFirstOrDefaultAsync(c => c.ProjectId == request.ProjectId);

            if (contract == null)
            {
                return new GetContractByProjectResponse { Success = false, Message = "No hay contrato para este proyecto" };
            }

            var contractDto = new ContractDto
            {
                ContractId = contract.ContractId,
                ProposalId = contract.ProposalId,
                ProjectId = contract.ProjectId,
                ClientId = contract.ClientId,
                FreelancerId = contract.FreelancerId,
                TotalAmount = contract.TotalAmount,
                ContractStatus = contract.ContractStatus,
                GeneratedAt = contract.GeneratedAt,
                ClientSignedAt = contract.ClientSignedAt,
                FreelancerSignedAt = contract.FreelancerSignedAt
            };

            return new GetContractByProjectResponse
            {
                Success = true,
                Message = "Contrato encontrado",
                Contract = contractDto
            };
        }
        catch (Exception ex)
        {
            return new GetContractByProjectResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }
}
