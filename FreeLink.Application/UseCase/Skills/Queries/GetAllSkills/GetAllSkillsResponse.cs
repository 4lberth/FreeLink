namespace FreeLink.Application.UseCase.Skills.Queries.GetAllSkills;

public class GetAllSkillsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SkillInfoDto> Skills { get; set; } = new();
}

public class SkillInfoDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}
