using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new DeleteUserResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Obtener el usuario a eliminar
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new DeleteUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // No permitir que un admin se elimine a sí mismo
            if (user.UserId == request.RequestingUserId)
            {
                return new DeleteUserResponse
                {
                    Success = false,
                    Message = "No puedes eliminarte a ti mismo"
                };
            }

            // Soft delete: marcar como inactivo
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complete();

            return new DeleteUserResponse
            {
                Success = true,
                Message = "Usuario eliminado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new DeleteUserResponse
            {
                Success = false,
                Message = $"Error al eliminar usuario: {ex.Message}"
            };
        }
    }
}
