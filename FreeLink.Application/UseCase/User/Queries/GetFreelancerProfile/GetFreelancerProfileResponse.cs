namespace FreeLink.Application.UseCase.User.Queries.GetFreelancerProfile;

public class FreelancerProfileDto
{
    public int FreelancerProfileId { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public decimal? HourlyRate { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? AvailabilityStatus { get; set; }
    public decimal? AverageRating { get; set; }
    public int? TotalReviews { get; set; }
    public List<SkillDto> Skills { get; set; } = new();
    public List<WorkExperienceDto> WorkExperiences { get; set; } = new();
    public List<PortfolioItemDto> PortfolioItems { get; set; } = new();
}

public class SkillDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ProficiencyLevel { get; set; }
}

public class WorkExperienceDto
{
    public int ExperienceId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string? Company { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? IsCurrent { get; set; }
    public string? Description { get; set; }
}

public class PortfolioItemDto
{
    public int PortfolioId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public List<PortfolioFileDto> Files { get; set; } = new();
}

public class PortfolioFileDto
{
    public int FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
}

public class GetFreelancerProfileResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public FreelancerProfileDto? Profile { get; set; }
}

