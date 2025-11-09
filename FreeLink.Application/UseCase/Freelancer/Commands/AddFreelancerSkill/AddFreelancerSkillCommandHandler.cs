using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Freelancer.Commands.AddFreelancerSkill;

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
            // 1. Validar permisos
            var isAdmin = request.RequestingUserRole == "Administrador";
            var isOwnProfile = request.RequestingUserId == request.FreelancerId;

            if (!isAdmin && !isOwnProfile)
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "No tienes permisos para agregar habilidades a este freelancer"
                };
            }

            // 2. Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.FreelancerId);

            if (user == null)
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 3. Verificar que sea freelancer
            if (user.UserType != "Freelancer")
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "Este usuario no es un freelancer"
                };
            }

            // 4. Verificar que la habilidad existe
            var skill = await _unitOfWork.Repository<Skill>()
                .GetById(request.SkillId);

            if (skill == null)
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "La habilidad especificada no existe"
                };
            }

            // 5. Verificar que el freelancer no tenga ya esta habilidad
            var existingSkill = await _unitOfWork.Repository<Freelancerskill>()
                .GetFirstOrDefaultAsync(fs => fs.UserId == request.FreelancerId && fs.SkillId == request.SkillId);

            if (existingSkill != null)
            {
                return new AddFreelancerSkillResponse
                {
                    Success = false,
                    Message = "El freelancer ya tiene esta habilidad agregada"
                };
            }

            // 6. Crear la relación freelancer-skill
            var freelancerSkill = new Freelancerskill
            {
                UserId = request.FreelancerId,
                SkillId = request.SkillId
            };

            await _unitOfWork.Repository<Freelancerskill>().Add(freelancerSkill);
            await _unitOfWork.Complete();

            // 7. Retornar respuesta exitosa
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