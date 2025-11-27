using FreeLink.Application.UseCase.WorkExperience.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Queries.GetUserWorkExperiences;

public class GetUserWorkExperiencesQuery : IRequest<GetUserWorkExperiencesResponse>
{
    public int UserId { get; set; }
}
