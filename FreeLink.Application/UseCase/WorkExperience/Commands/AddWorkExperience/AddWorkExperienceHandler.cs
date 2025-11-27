using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Commands.AddWorkExperience;

public class AddWorkExperienceHandler : IRequestHandler<AddWorkExperienceCommand, AddWorkExperienceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddWorkExperienceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AddWorkExperienceResponse> Handle(AddWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el usuario existe
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new AddWorkExperienceResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Validar que solo el dueño puede agregar experiencia a su perfil
            if (request.RequestingUserId != request.UserId)
            {
                return new AddWorkExperienceResponse
                {
                    Success = false,
                    Message = "No tienes permiso para agregar experiencia a este perfil"
                };
            }

            // Validar fechas
            if (request.EndDate.HasValue && request.EndDate < request.StartDate)
            {
                return new AddWorkExperienceResponse
                {
                    Success = false,
                    Message = "La fecha de fin no puede ser anterior a la fecha de inicio"
                };
            }

            // Crear nueva experiencia
            var workExperience = new Workexperience
            {
                UserId = request.UserId,
                JobTitle = request.JobTitle,
                Company = request.Company,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsCurrent = request.IsCurrent,
                Description = request.Description
            };

            await _unitOfWork.Repository<Workexperience>().Add(workExperience);
            await _unitOfWork.Complete();

            return new AddWorkExperienceResponse
            {
                Success = true,
                Message = "Experiencia laboral agregada exitosamente",
                ExperienceId = workExperience.ExperienceId
            };
        }
        catch (Exception ex)
        {
            return new AddWorkExperienceResponse
            {
                Success = false,
                Message = $"Error al agregar experiencia: {ex.Message}"
            };
        }
    }
}
