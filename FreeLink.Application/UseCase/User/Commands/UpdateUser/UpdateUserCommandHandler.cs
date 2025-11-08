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
            // 1. Validar permisos
            var isAdmin = request.RequestingUserRole == "Administrador";
            var isOwnProfile = request.RequestingUserId == request.UserId;

            if (!isAdmin && !isOwnProfile)
            {
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = "No tienes permisos para actualizar este usuario"
                };
            }

            // 2. Buscar usuario
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.UserId);

            if (user == null)
            {
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 3. Validar cambio de email (si se proporciona)
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var emailExists = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                    .AnyAsync(u => u.Email == request.Email && u.UserId != request.UserId);

                if (emailExists)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "El email ya está registrado por otro usuario"
                    };
                }

                user.Email = request.Email;
                user.IsVerified = false; // Requiere verificación de nuevo
            }

            // 4. Actualizar UserType (solo admin)
            if (!string.IsNullOrEmpty(request.UserType))
            {
                if (!isAdmin)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "Solo los administradores pueden cambiar el tipo de usuario"
                    };
                }

                var validUserTypes = new[] { "Cliente", "Freelancer", "Administrador" };
                if (!validUserTypes.Contains(request.UserType))
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "Tipo de usuario inválido"
                    };
                }

                user.UserType = request.UserType;
            }

            // 5. Actualizar IsActive (solo admin)
            if (request.IsActive.HasValue)
            {
                if (!isAdmin)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "Solo los administradores pueden activar/desactivar usuarios"
                    };
                }

                user.IsActive = request.IsActive.Value;
            }

            // 6. Actualizar IsVerified (solo admin)
            if (request.IsVerified.HasValue)
            {
                if (!isAdmin)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "Solo los administradores pueden verificar usuarios"
                    };
                }

                user.IsVerified = request.IsVerified.Value;
            }

            // 7. Actualizar fecha de modificación
            user.UpdatedAt = DateTime.UtcNow;

            // 8. Guardar cambios
            await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);
            await _unitOfWork.Complete();

            // 9. Retornar respuesta exitosa
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