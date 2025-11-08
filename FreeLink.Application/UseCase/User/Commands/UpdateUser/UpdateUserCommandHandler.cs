using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            
            if (user == null)
            {
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Actualizar email si se proporciona
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                // Verificar que el nuevo email no esté en uso
                var emailExists = await _unitOfWork.Repository<Domain.Entities.User>()
                    .AnyAsync(u => u.Email == request.Email && u.UserId != request.UserId);
                
                if (emailExists)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "El email ya está en uso por otro usuario"
                    };
                }
                
                user.Email = request.Email;
            }

            // 3. Actualizar IsActive si se proporciona
            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Domain.Entities.User>().Update(user);

            // 4. Actualizar perfil de usuario si existe
            var userProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(up => up.UserId == request.UserId);

            if (userProfile != null)
            {
                if (!string.IsNullOrEmpty(request.FirstName))
                    userProfile.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    userProfile.LastName = request.LastName;
                
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    userProfile.PhoneNumber = request.PhoneNumber;
                
                if (!string.IsNullOrEmpty(request.Country))
                    userProfile.Country = request.Country;
                
                if (!string.IsNullOrEmpty(request.City))
                    userProfile.City = request.City;
                
                if (!string.IsNullOrEmpty(request.Bio))
                    userProfile.Bio = request.Bio;
                
                if (!string.IsNullOrEmpty(request.ProfilePicture))
                    userProfile.ProfilePicture = request.ProfilePicture;

                await _unitOfWork.Repository<Userprofile>().Update(userProfile);
            }
            else if (request.FirstName != null || request.LastName != null)
            {
                // Crear perfil si no existe pero se proporcionan datos
                userProfile = new Userprofile
                {
                    UserId = request.UserId,
                    FirstName = request.FirstName ?? string.Empty,
                    LastName = request.LastName ?? string.Empty,
                    PhoneNumber = request.PhoneNumber,
                    Country = request.Country,
                    City = request.City,
                    Bio = request.Bio,
                    ProfilePicture = request.ProfilePicture
                };
                await _unitOfWork.Repository<Userprofile>().Add(userProfile);
            }

            await _unitOfWork.Complete();

            return new UpdateUserResponse
            {
                Success = true,
                Message = "Usuario actualizado exitosamente",
                UserId = user.UserId
            };
        }
        catch (Exception ex)
        {
            return new UpdateUserResponse
            {
                Success = false,
                Message = $"Error al actualizar usuario: {ex.Message}"
            };
        }
    }
}

