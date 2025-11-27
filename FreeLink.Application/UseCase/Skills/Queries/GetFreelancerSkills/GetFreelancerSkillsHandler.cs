using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Skills.Queries.GetFreelancerSkills;

public class GetFreelancerSkillsHandler : IRequestHandler<GetFreelancerSkillsQuery, GetFreelancerSkillsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFreelancerSkillsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetFreelancerSkillsResponse> Handle(GetFreelancerSkillsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el freelancer existe (buscar por UserId, no por FreelancerId)
            var freelancer = await _unitOfWork.Repository<Freelancerprofile>()
                .GetFirstOrDefaultAsync(fp => fp.UserId == request.FreelancerId);
            
            if (freelancer == null)
            {
                return new GetFreelancerSkillsResponse
                {
                    Success = false,
                    Message = "Freelancer no encontrado"
                };
            }

            // Obtener las habilidades del freelancer
            var freelancerSkills = await _unitOfWork.Repository<Freelancerskill>()
                .GetAsync(fs => fs.UserId == request.FreelancerId);

            var skillsList = new List<SkillDto>();

            foreach (var fs in freelancerSkills)
            {
                var skill = await _unitOfWork.Repository<Skill>().GetById(fs.SkillId);
                if (skill != null)
                {
                    skillsList.Add(new SkillDto
                    {
                        SkillId = skill.SkillId,
                        SkillName = skill.SkillName,
                        Category = skill.Category,
                        ExperienceLevel = fs.ProficiencyLevel
                    });
                }
            }

            return new GetFreelancerSkillsResponse
            {
                Success = true,
                Message = "Habilidades obtenidas exitosamente",
                Skills = skillsList
            };
        }
        catch (Exception ex)
        {
            return new GetFreelancerSkillsResponse
            {
                Success = false,
                Message = $"Error al obtener habilidades: {ex.Message}"
            };
        }
    }
}
