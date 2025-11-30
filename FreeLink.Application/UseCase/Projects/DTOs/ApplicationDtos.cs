namespace FreeLink.Application.UseCase.Projects.DTOs;

public class ApplicationDto
{
    public int ApplicationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public int FreelancerId { get; set; }
    public string FreelancerName { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public decimal? ProposedRate { get; set; }
    public int? EstimatedDuration { get; set; }
    public string ApplicationStatus { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public class CreateApplicationRequest
{
    public string? CoverLetter { get; set; }
    public decimal? ProposedRate { get; set; }
    public int? EstimatedDuration { get; set; }
}
