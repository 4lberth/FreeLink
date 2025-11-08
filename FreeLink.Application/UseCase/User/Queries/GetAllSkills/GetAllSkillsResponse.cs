namespace FreeLink.Application.UseCase.User.Queries.GetAllSkills;

public class SkillResponseDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class GetAllSkillsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SkillResponseDto> Skills { get; set; } = new();
}

