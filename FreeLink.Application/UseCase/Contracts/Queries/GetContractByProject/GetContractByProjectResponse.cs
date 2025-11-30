using FreeLink.Application.UseCase.Contracts.DTOs;

namespace FreeLink.Application.UseCase.Contracts.Queries.GetContractByProject;

public class GetContractByProjectResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ContractDto? Contract { get; set; }
}
