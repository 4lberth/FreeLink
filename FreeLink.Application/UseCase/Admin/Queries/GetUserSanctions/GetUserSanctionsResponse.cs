namespace FreeLink.Application.UseCase.Admin.Queries.GetUserSanctions;

public class GetUserSanctionsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SanctionDto> Sanctions { get; set; } = new();
}

public class SanctionDto
{
    public int SanctionId { get; set; }
    public string? SanctionType { get; set; }
    public string? Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
    public int AppliedBy { get; set; }
    public string AppliedByName { get; set; } = string.Empty;
}
