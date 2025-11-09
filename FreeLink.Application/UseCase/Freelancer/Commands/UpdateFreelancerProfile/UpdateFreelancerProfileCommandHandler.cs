using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Freelancer.Commands.UpdateFreelancerProfile;

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
            // 1. Validar permisos
            var isAdmin = request.RequestingUserRole == "Administrador";
            var isOwnProfile = request.RequestingUserId == request.FreelancerId;

            if (!isAdmin && !isOwnProfile)
            {
                return new UpdateFreelancerProfileResponse
                {
                    Success = false,
                    Message = "No tienes permisos para actualizar este perfil de freelancer"
                };
            }

            // 2. Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.FreelancerId);

            if (user == null)
            {
                return new UpdateFreelancerProfileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 3. Verificar que sea freelancer
            if (user.UserType != "Freelancer")
            {
                return new UpdateFreelancerProfileResponse
                {
                    Success = false,
                    Message = "Este usuario no es un freelancer"
                };
            }

            // 4. Buscar perfil profesional del freelancer
            var freelancerProfile = await _unitOfWork.Repository<Freelancerprofile>()
                .GetFirstOrDefaultAsync(fp => fp.UserId == request.FreelancerId);

            if (freelancerProfile == null)
            {
                return new UpdateFreelancerProfileResponse
                {
                    Success = false,
                    Message = "Perfil de freelancer no encontrado"
                };
            }

            // 5. Validar hourlyRate (si se proporciona)
            if (request.HourlyRate.HasValue)
            {
                if (request.HourlyRate.Value < 0)
                {
                    return new UpdateFreelancerProfileResponse
                    {
                        Success = false,
                        Message = "La tarifa por hora no puede ser negativa"
                    };
                }

                if (request.HourlyRate.Value > 10000)
                {
                    return new UpdateFreelancerProfileResponse
                    {
                        Success = false,
                        Message = "La tarifa por hora no puede exceder $10,000"
                    };
                }

                freelancerProfile.HourlyRate = request.HourlyRate.Value;
            }

            // 6. Validar availabilityStatus (si se proporciona)
            if (!string.IsNullOrEmpty(request.AvailabilityStatus))
            {
                var validStatuses = new[] { "Disponible", "No disponible", "Ocupado" };
                
                if (!validStatuses.Contains(request.AvailabilityStatus))
                {
                    return new UpdateFreelancerProfileResponse
                    {
                        Success = false,
                        Message = "Estado de disponibilidad inválido. Valores permitidos: Disponible, No disponible, Ocupado"
                    };
                }

                freelancerProfile.AvailabilityStatus = request.AvailabilityStatus;
            }

            // 7. Actualizar professionalTitle (si se proporciona)
            if (request.ProfessionalTitle != null)
            {
                if (request.ProfessionalTitle.Length > 100)
                {
                    return new UpdateFreelancerProfileResponse
                    {
                        Success = false,
                        Message = "El título profesional no puede exceder 100 caracteres"
                    };
                }

                freelancerProfile.Title = request.ProfessionalTitle;
            }

            // 8. Guardar cambios
            await _unitOfWork.Repository<Freelancerprofile>().Update(freelancerProfile);
            await _unitOfWork.Complete();

            // 9. Retornar respuesta exitosa
            return new UpdateFreelancerProfileResponse
            {
                Success = true,
                Message = "Perfil profesional actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateFreelancerProfileResponse
            {
                Success = false,
                Message = $"Error al actualizar perfil profesional: {ex.Message}"
            };
        }
    }
}