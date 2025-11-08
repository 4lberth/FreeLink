using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateUserProfileResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar permisos
            var isAdmin = request.RequestingUserRole == "Administrador";
            var isOwnProfile = request.RequestingUserId == request.UserId;

            if (!isAdmin && !isOwnProfile)
            {
                return new UpdateUserProfileResponse
                {
                    Success = false,
                    Message = "No tienes permisos para actualizar este perfil"
                };
            }

            // 2. Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.UserId);

            if (user == null)
            {
                return new UpdateUserProfileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 3. Buscar el perfil del usuario
            var userProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(up => up.UserId == request.UserId);

            if (userProfile == null)
            {
                return new UpdateUserProfileResponse
                {
                    Success = false,
                    Message = "Perfil no encontrado"
                };
            }

            // 4. Actualizar campos (solo si se proporcionan)
            if (!string.IsNullOrEmpty(request.FirstName))
            {
                userProfile.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                userProfile.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                userProfile.PhoneNumber = request.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(request.Country))
            {
                userProfile.Country = request.Country;
            }

            if (!string.IsNullOrEmpty(request.City))
            {
                userProfile.City = request.City;
            }

            if (request.Bio != null) // Permitir vacío
            {
                userProfile.Bio = request.Bio;
            }

            if (request.ProfilePictureUrl != null) // Permitir vacío
            {
                userProfile.ProfilePicture = request.ProfilePictureUrl;
            }

            // 5. Guardar cambios
            await _unitOfWork.Repository<Userprofile>().Update(userProfile);
            await _unitOfWork.Complete();

            // 6. Retornar respuesta exitosa
            return new UpdateUserProfileResponse
            {
                Success = true,
                Message = "Perfil actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateUserProfileResponse
            {
                Success = false,
                Message = $"Error al actualizar perfil: {ex.Message}"
            };
        }
    }
}