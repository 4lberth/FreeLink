using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ActivateUser;

public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, ActivateUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ActivateUserResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new ActivateUserResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Obtener el usuario a activar
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new ActivateUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Verificar si ya está activo
            if (user.IsActive == true)
            {
                return new ActivateUserResponse
                {
                    Success = false,
                    Message = "El usuario ya está activo"
                };
            }

            // Activar usuario
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complete();

            return new ActivateUserResponse
            {
                Success = true,
                Message = "Usuario activado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ActivateUserResponse
            {
                Success = false,
                Message = $"Error al activar usuario: {ex.Message}"
            };
        }
    }
}
