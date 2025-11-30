namespace FreeLink.Application.UseCase.Contracts.Commands.SignContract;

public class SignContractResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool BothSigned { get; set; }
    public string? PdfUrl { get; set; }
}
