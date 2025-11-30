namespace FreeLink.Application.UseCase.Projects.DTOs;

public class ProjectDto
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateOnly DeadlineDate { get; set; }
    public string ProjectStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    
    // Cliente
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    
    // Freelancer asignado (opcional)
    public int? AssignedFreelancerId { get; set; }
    public string? AssignedFreelancerName { get; set; }
    
    // Skills requeridas
    public List<SkillDto> RequiredSkills { get; set; } = new();
    
    // Conteo de aplicaciones (solo para cliente dueño)
    public int? ApplicationCount { get; set; }
}

public class ProjectSummaryDto
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateOnly DeadlineDate { get; set; }
    public string ProjectStatus { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public List<SkillDto> RequiredSkills { get; set; } = new();
}

public class SkillDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class CreateProjectRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateOnly DeadlineDate { get; set; }
    public List<int> RequiredSkillIds { get; set; } = new();
}

public class UpdateProjectRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateOnly DeadlineDate { get; set; }
    public List<int> RequiredSkillIds { get; set; } = new();
}
