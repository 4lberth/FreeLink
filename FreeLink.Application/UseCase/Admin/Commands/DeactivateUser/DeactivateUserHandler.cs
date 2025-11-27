using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.DeactivateUser;

public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, DeactivateUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeactivateUserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new DeactivateUserResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Obtener el usuario a desactivar
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new DeactivateUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // No permitir que un admin se desactive a sí mismo
            if (user.UserId == request.RequestingUserId)
            {
                return new DeactivateUserResponse
                {
                    Success = false,
                    Message = "No puedes desactivarte a ti mismo"
                };
            }

            // Verificar si ya está inactivo
            if (user.IsActive == false)
            {
                return new DeactivateUserResponse
                {
                    Success = false,
                    Message = "El usuario ya está inactivo"
                };
            }

            // Desactivar usuario
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complete();

            return new DeactivateUserResponse
            {
                Success = true,
                Message = "Usuario desactivado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DeactivateUserResponse
            {
                Success = false,
                Message = $"Error al desactivar usuario: {ex.Message}"
            };
        }
    }
}
