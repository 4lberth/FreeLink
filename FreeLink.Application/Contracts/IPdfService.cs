namespace FreeLink.Application.Contracts;

public interface IPdfService
{
    Task<string> GenerateContractPdfAsync(int contractId);
}