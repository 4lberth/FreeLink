using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Skills.Queries.GetAllSkills;

public class GetAllSkillsHandler : IRequestHandler<GetAllSkillsQuery, GetAllSkillsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSkillsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllSkillsResponse> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skills = await _unitOfWork.Repository<Skill>().GetAll();

            var skillsList = skills
                .OrderBy(s => s.Category)
                .ThenBy(s => s.SkillName)
                .Select(s => new SkillInfoDto
                {
                    SkillId = s.SkillId,
                    SkillName = s.SkillName,
                    Category = s.Category
                })
                .ToList();

            return new GetAllSkillsResponse
            {
                Success = true,
                Message = "Habilidades obtenidas exitosamente",
                Skills = skillsList
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
