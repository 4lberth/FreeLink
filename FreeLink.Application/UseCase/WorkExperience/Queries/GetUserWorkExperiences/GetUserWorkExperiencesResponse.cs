using FreeLink.Application.UseCase.WorkExperience.DTOs;

namespace FreeLink.Application.UseCase.WorkExperience.Queries.GetUserWorkExperiences;

public class GetUserWorkExperiencesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<WorkExperienceDto> Experiences { get; set; } = new();
}
