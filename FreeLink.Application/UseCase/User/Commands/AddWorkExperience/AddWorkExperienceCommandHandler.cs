using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.AddWorkExperience;

public class AddWorkExperienceCommandHandler : IRequestHandler<AddWorkExperienceCommand, AddWorkExperienceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddWorkExperienceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AddWorkExperienceResponse> Handle(AddWorkExperienceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            
            if (user == null)
            {
                return new AddWorkExperienceResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

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
                Message = $"Error al agregar experiencia laboral: {ex.Message}"
            };
        }
    }
}

