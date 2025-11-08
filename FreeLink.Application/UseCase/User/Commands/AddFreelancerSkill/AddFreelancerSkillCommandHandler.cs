using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.AddFreelancerSkill;

public class AddFreelancerSkillCommandHandler : IRequestHandler<AddFreelancerSkillCommand, AddFreelancerSkillResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddFreelancerSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AddFreelancerSkillResponse> Handle(AddFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el usuario existe y es Freelancer
            var user = await _unitOfWork.Repository<User>().GetById(request.UserId);
            
            if (user == null || user.UserType != "Freelancer")
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado o no es un Freelancer"
                };
            }

            // 2. Verificar que la habilidad existe
            var skill = await _unitOfWork.Repository<Skill>().GetById(request.SkillId);
            
            if (skill == null)
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Habilidad no encontrada"
                };
            }

            // 3. Verificar que no tenga ya esta habilidad
            var existingSkill = await _unitOfWork.Repository<Freelancerskill>()
                .GetFirstOrDefaultAsync(fs => fs.UserId == request.UserId && fs.SkillId == request.SkillId);
            
            if (existingSkill != null)
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "El freelancer ya tiene esta habilidad registrada"
                };
            }

            // 4. Agregar habilidad
            var freelancerSkill = new Freelancerskill
            {
                UserId = request.UserId,
                SkillId = request.SkillId,
                ProficiencyLevel = request.ProficiencyLevel ?? "Intermedio"
            };

            await _unitOfWork.Repository<Freelancerskill>().Add(freelancerSkill);
            await _unitOfWork.Complete();

            return new AddFreelancerSkillResponse
            {
                Success = true,
                Message = "Habilidad agregada exitosamente",
                FreelancerSkillId = freelancerSkill.FreelancerSkillId
            };
        }
        catch (Exception ex)
        {
            return new AddFreelancerSkillResponse
            {
                Success = false,
                Message = $"Error al agregar habilidad: {ex.Message}"
            };
        }
    }
}

