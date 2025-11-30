using MediatR;

namespace FreeLink.Application.UseCase.Contracts.Commands.SignContract;

public class SignContractCommand : IRequest<SignContractResponse>
{
    public int ContractId { get; set; }
    public int UserId { get; set; }
    public string? IpAddress { get; set; }
}
