using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.AddFreelancerSkill;

public class AddFreelancerSkillCommand : IRequest<AddFreelancerSkillResponse>
{
    public int UserId { get; set; }
    public int SkillId { get; set; }
    public string? ProficiencyLevel { get; set; } // Básico, Intermedio, Avanzado, Experto
}

