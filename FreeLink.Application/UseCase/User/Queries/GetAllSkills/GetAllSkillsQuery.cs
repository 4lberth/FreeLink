using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetAllSkills;

public class GetAllSkillsQuery : IRequest<GetAllSkillsResponse>
{
    public string? Category { get; set; }
}

