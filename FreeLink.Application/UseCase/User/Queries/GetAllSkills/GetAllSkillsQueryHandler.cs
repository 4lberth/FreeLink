using AutoMapper;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetAllSkills;

public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, GetAllSkillsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSkillsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllSkillsResponse> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Skill> skills;

            if (!string.IsNullOrEmpty(request.Category))
            {
                skills = await _unitOfWork.Repository<Skill>()
                    .GetAsync(s => s.Category == request.Category);
            }
            else
            {
                skills = await _unitOfWork.Repository<Skill>().GetAll();
            }

            var skillsDto = skills.Select(s => new SkillResponseDto
            {
                SkillId = s.SkillId,
                SkillName = s.SkillName,
                Category = s.Category
            }).ToList();

            return new GetAllSkillsResponse
            {
                Success = true,
                Message = "Habilidades obtenidas exitosamente",
                Skills = skillsDto
            };
        }
        catch (Exception ex)
        {
            return new GetAllSkillsResponse
            {
                Success = false,
                Message = $"Error al obtener habilidades: {ex.Message}"
            };
        }
    }
}

