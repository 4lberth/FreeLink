using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Commands.DeleteWorkExperience;

public class DeleteWorkExperienceHandler : IRequestHandler<DeleteWorkExperienceCommand, DeleteWorkExperienceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWorkExperienceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteWorkExperienceResponse> Handle(DeleteWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener la experiencia
            var experience = await _unitOfWork.Repository<Workexperience>().GetById(request.ExperienceId);
            if (experience == null)
            {
                return new DeleteWorkExperienceResponse
                {
                    Success = false,
                    Message = "Experiencia laboral no encontrada"
                };
            }

            // Validar que solo el dueño puede eliminar
            if (experience.UserId != request.RequestingUserId)
            {
                return new DeleteWorkExperienceResponse
                {
                    Success = false,
                    Message = "No tienes permiso para eliminar esta experiencia"
                };
            }

            await _unitOfWork.Repository<Workexperience>().Delete(request.ExperienceId);
            await _unitOfWork.Complete();

            return new DeleteWorkExperienceResponse
            {
                Success = true,
                Message = "Experiencia laboral eliminada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DeleteWorkExperienceResponse
            {
                Success = false,
                Message = $"Error al eliminar experiencia: {ex.Message}"
            };
        }
    }
}
