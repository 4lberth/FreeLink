namespace FreeLink.Application.UseCase.Skills.Queries.GetFreelancerSkills;

public class GetFreelancerSkillsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = new();
}

public class SkillDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ExperienceLevel { get; set; }
}
