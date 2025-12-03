namespace FreeLink.Application.UseCase.Admin.Queries.GetAllSanctions;

public class GetAllSanctionsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SanctionListDto> Sanctions { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class SanctionListDto
{
    public int SanctionId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? SanctionType { get; set; }
    public string? Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
    public int AppliedBy { get; set; }
    public string AppliedByName { get; set; } = string.Empty;
}
