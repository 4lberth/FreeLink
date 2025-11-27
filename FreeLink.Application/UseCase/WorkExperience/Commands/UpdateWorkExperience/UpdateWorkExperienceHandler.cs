using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Commands.UpdateWorkExperience;

public class UpdateWorkExperienceHandler : IRequestHandler<UpdateWorkExperienceCommand, UpdateWorkExperienceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkExperienceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateWorkExperienceResponse> Handle(UpdateWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener la experiencia existente
            var experience = await _unitOfWork.Repository<Workexperience>().GetById(request.ExperienceId);
            if (experience == null)
            {
                return new UpdateWorkExperienceResponse
                {
                    Success = false,
                    Message = "Experiencia laboral no encontrada"
                };
            }

            // Validar que solo el dueño puede actualizar
            if (experience.UserId != request.RequestingUserId)
            {
                return new UpdateWorkExperienceResponse
                {
                    Success = false,
                    Message = "No tienes permiso para actualizar esta experiencia"
                };
            }

            // Validar fechas
            if (request.EndDate.HasValue && request.EndDate < request.StartDate)
            {
                return new UpdateWorkExperienceResponse
                {
                    Success = false,
                    Message = "La fecha de fin no puede ser anterior a la fecha de inicio"
                };
            }

            // Actualizar campos
            experience.JobTitle = request.JobTitle;
            experience.Company = request.Company;
            experience.StartDate = request.StartDate;
            experience.EndDate = request.EndDate;
            experience.IsCurrent = request.IsCurrent;
            experience.Description = request.Description;

            await _unitOfWork.Repository<Workexperience>().Update(experience);
            await _unitOfWork.Complete();

            return new UpdateWorkExperienceResponse
            {
                Success = true,
                Message = "Experiencia laboral actualizada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateWorkExperienceResponse
            {
                Success = false,
                Message = $"Error al actualizar experiencia: {ex.Message}"
            };
        }
    }
}
