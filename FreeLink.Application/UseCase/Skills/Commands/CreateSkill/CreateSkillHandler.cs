using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Skills.Commands.CreateSkill;

public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, CreateSkillResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSkillHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateSkillResponse> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new CreateSkillResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Validar que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(request.SkillName))
            {
                return new CreateSkillResponse
                {
                    Success = false,
                    Message = "El nombre de la habilidad es requerido"
                };
            }

            // Verificar si ya existe una skill con ese nombre
            var existingSkill = (await _unitOfWork.Repository<Skill>()
                .GetAsync(s => s.SkillName.ToLower() == request.SkillName.ToLower()))
                .FirstOrDefault();

            if (existingSkill != null)
            {
                return new CreateSkillResponse
                {
                    Success = false,
                    Message = "Ya existe una habilidad con ese nombre"
                };
            }

            // Crear nueva skill
            var newSkill = new Skill
            {
                SkillName = request.SkillName.Trim(),
                Category = request.Category?.Trim()
            };

            await _unitOfWork.Repository<Skill>().Add(newSkill);
            await _unitOfWork.Complete();

            return new CreateSkillResponse
            {
                Success = true,
                Message = "Habilidad creada exitosamente",
                SkillId = newSkill.SkillId
            };
        }
        catch (Exception ex)
        {
            return new CreateSkillResponse
            {
                Success = false,
                Message = $"Error al crear habilidad: {ex.Message}"
            };
        }
    }
}
