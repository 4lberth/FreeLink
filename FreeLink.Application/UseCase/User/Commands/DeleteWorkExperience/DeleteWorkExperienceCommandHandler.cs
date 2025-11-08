using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.DeleteWorkExperience;

public class DeleteWorkExperienceCommandHandler : IRequestHandler<DeleteWorkExperienceCommand, DeleteWorkExperienceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWorkExperienceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteWorkExperienceResponse> Handle(DeleteWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var workExperience = await _unitOfWork.Repository<Workexperience>().GetById(request.ExperienceId);
            
            if (workExperience == null)
            {
                return new DeleteWorkExperienceResponse
                {
                    Success = false,
                    Message = "Experiencia laboral no encontrada"
                };
            }

            if (workExperience.UserId != request.UserId)
            {
                return new DeleteWorkExperienceResponse
                {
                    Success = false,
                    Message = "No tiene permiso para eliminar esta experiencia laboral"
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
                Message = $"Error al eliminar experiencia laboral: {ex.Message}"
            };
        }
    }
}

