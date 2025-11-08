using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler : IRequestHandler<UpdateFreelancerProfileCommand, UpdateFreelancerProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFreelancerProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateFreelancerProfileResponse> Handle(UpdateFreelancerProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el usuario existe y es Freelancer
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            
            if (user == null)
            {
                return new UpdateFreelancerProfileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            if (user.UserType != "Freelancer")
            {
                return new UpdateFreelancerProfileResponse
                {
                    Success = false,
                    Message = "El usuario no es un Freelancer"
                };
            }

            // 2. Buscar o crear perfil de freelancer
            var freelancerProfile = await _unitOfWork.Repository<Freelancerprofile>()
                .GetFirstOrDefaultAsync(fp => fp.UserId == request.UserId);

            if (freelancerProfile == null)
            {
                // Crear nuevo perfil
                freelancerProfile = new Freelancerprofile
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    HourlyRate = request.HourlyRate,
                    YearsOfExperience = request.YearsOfExperience,
                    AvailabilityStatus = request.AvailabilityStatus ?? "Disponible",
                    TotalEarnings = 0,
                    CompletedProjects = 0,
                    AverageRating = 0,
                    TotalReviews = 0
                };
                await _unitOfWork.Repository<Freelancerprofile>().Add(freelancerProfile);
            }
            else
            {
                // Actualizar perfil existente
                if (!string.IsNullOrEmpty(request.Title))
                    freelancerProfile.Title = request.Title;
                
                if (request.HourlyRate.HasValue)
                    freelancerProfile.HourlyRate = request.HourlyRate.Value;
                
                if (request.YearsOfExperience.HasValue)
                    freelancerProfile.YearsOfExperience = request.YearsOfExperience.Value;
                
                if (!string.IsNullOrEmpty(request.AvailabilityStatus))
                    freelancerProfile.AvailabilityStatus = request.AvailabilityStatus;

                await _unitOfWork.Repository<Freelancerprofile>().Update(freelancerProfile);
            }

            await _unitOfWork.Complete();

            return new UpdateFreelancerProfileResponse
            {
                Success = true,
                Message = "Perfil de freelancer actualizado exitosamente",
                FreelancerProfileId = freelancerProfile.FreelancerProfileId
            };
        }
        catch (Exception ex)
        {
            return new UpdateFreelancerProfileResponse
            {
                Success = false,
                Message = $"Error al actualizar perfil de freelancer: {ex.Message}"
            };
        }
    }
}

