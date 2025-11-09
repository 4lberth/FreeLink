using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar permisos
            var isAdmin = request.RequestingUserRole == "Administrador";
            var isOwnAccount = request.RequestingUserId == request.UserId;

            if (!isAdmin && !isOwnAccount)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "No tienes permisos para cambiar la contraseña de este usuario"
                };
            }

            // 2. Validar que las contraseñas nuevas coincidan
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Las contraseñas nuevas no coinciden"
                };
            }

            // 3. Validar longitud mínima de la contraseña
            if (request.NewPassword.Length < 6)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "La contraseña debe tener al menos 6 caracteres"
                };
            }

            // 4. Buscar usuario
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.UserId);

            if (user == null)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 5. Si NO es admin, verificar contraseña actual
            if (!isAdmin)
            {
                if (string.IsNullOrEmpty(request.CurrentPassword))
                {
                    return new ChangePasswordResponse
                    {
                        Success = false,
                        Message = "Debes proporcionar tu contraseña actual"
                    };
                }

                // Verificar contraseña actual
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    return new ChangePasswordResponse
                    {
                        Success = false,
                        Message = "La contraseña actual es incorrecta"
                    };
                }
            }

            // 6. Validar que la nueva contraseña sea diferente a la actual
            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "La nueva contraseña debe ser diferente a la actual"
                };
            }

            // 7. Hashear la nueva contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // 8. Guardar cambios
            await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);
            await _unitOfWork.Complete();

            // 9. Retornar respuesta exitosa
            var message = isAdmin && !isOwnAccount 
                ? "Contraseña restablecida exitosamente por el administrador" 
                : "Contraseña actualizada exitosamente";

            return new ChangePasswordResponse
            {
                Success = true,
                Message = message
            };
        }
        catch (Exception ex)
        {
            return new ChangePasswordResponse
            {
                Success = false,
                Message = $"Error al cambiar contraseña: {ex.Message}"
            };
        }
    }
}