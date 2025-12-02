namespace FreeLink.Domain.Ports;

public interface IPdfService
{
    Task<string> GenerateContractPdfAsync(int contractId);
    byte[] GeneratePdfFromHtml(string htmlContent);
}
