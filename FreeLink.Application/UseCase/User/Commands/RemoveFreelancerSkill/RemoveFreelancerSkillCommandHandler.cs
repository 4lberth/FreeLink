using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.RemoveFreelancerSkill;

public class RemoveFreelancerSkillCommandHandler : IRequestHandler<RemoveFreelancerSkillCommand, RemoveFreelancerSkillResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFreelancerSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RemoveFreelancerSkillResponse> Handle(RemoveFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var freelancerSkill = await _unitOfWork.Repository<Freelancerskill>()
                .GetFirstOrDefaultAsync(fs => fs.UserId == request.UserId && fs.SkillId == request.SkillId);
            
            if (freelancerSkill == null)
            {
                return new RemoveFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Habilidad no encontrada para este freelancer"
                };
            }

            await _unitOfWork.Repository<Freelancerskill>().Delete(freelancerSkill.FreelancerSkillId);
            await _unitOfWork.Complete();

            return new RemoveFreelancerSkillResponse
            {
                Success = true,
                Message = "Habilidad eliminada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new RemoveFreelancerSkillResponse
            {
                Success = false,
                Message = $"Error al eliminar habilidad: {ex.Message}"
            };
        }
    }
}

