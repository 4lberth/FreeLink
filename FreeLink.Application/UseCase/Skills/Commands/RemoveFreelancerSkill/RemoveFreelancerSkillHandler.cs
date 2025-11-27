using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Skills.Commands.RemoveFreelancerSkill;

public class RemoveFreelancerSkillHandler : IRequestHandler<RemoveFreelancerSkillCommand, RemoveFreelancerSkillResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFreelancerSkillHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RemoveFreelancerSkillResponse> Handle(RemoveFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el freelancer existe (buscar por UserId, no por FreelancerId)
            var freelancer = await _unitOfWork.Repository<Freelancerprofile>()
                .GetFirstOrDefaultAsync(fp => fp.UserId == request.FreelancerId);
            
            if (freelancer == null)
            {
                return new RemoveFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Freelancer no encontrado"
                };
            }

            // Validar que solo el dueño puede eliminar sus skills
            // request.FreelancerId es el UserId del freelancer (de la ruta /api/freelancers/{id})
            // request.RequestingUserId es el UserId del usuario autenticado (del token)
            if (request.FreelancerId != request.RequestingUserId)
            {
                return new RemoveFreelancerSkillResponse
                {
                    Success = false,
                    Message = "No tienes permiso para modificar estas habilidades"
                };
            }

            // Buscar la relación FreelancerSkill
            var freelancerSkill = (await _unitOfWork.Repository<Freelancerskill>()
                .GetAsync(fs => fs.UserId == request.FreelancerId && fs.SkillId == request.SkillId))
                .FirstOrDefault();

            if (freelancerSkill == null)
            {
                return new RemoveFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Habilidad no encontrada en el perfil del freelancer"
                };
            }

            // Eliminar la habilidad (pasamos el ID, no el objeto)
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
