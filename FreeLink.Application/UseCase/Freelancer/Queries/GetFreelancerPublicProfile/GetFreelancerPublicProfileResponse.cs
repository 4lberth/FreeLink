namespace FreeLink.Application.UseCase.Freelancer.Queries.GetFreelancerPublicProfile;

public class GetFreelancerPublicProfileResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public FreelancerPublicProfileDto? Profile { get; set; }
}

public class FreelancerPublicProfileDto
{
    // Información básica del usuario
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    
    // Información profesional del freelancer
    public string AvailabilityStatus { get; set; } = string.Empty;
    public decimal? HourlyRate { get; set; }
    public int CompletedProjects { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    
    // Habilidades
    public List<SkillDto> Skills { get; set; } = new();
    
    // Experiencia laboral
    public List<WorkExperienceDto> Experience { get; set; } = new();
    
    // Portafolio
    public List<PortfolioItemDto> Portfolio { get; set; } = new();
}

public class SkillDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class WorkExperienceDto
{
    public int ExperienceId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsCurrentJob { get; set; }
}

public class PortfolioItemDto
{
    public int PortfolioId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? Technologies { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PortfolioFileDto> Files { get; set; } = new();
}

public class PortfolioFileDto
{
    public int FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}