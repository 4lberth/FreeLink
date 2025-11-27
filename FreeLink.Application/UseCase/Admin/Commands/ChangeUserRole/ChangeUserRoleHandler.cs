using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ChangeUserRole;

public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand, ChangeUserRoleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private static readonly string[] ValidRoles = { "Cliente", "Freelancer", "Administrador" };

    public ChangeUserRoleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeUserRoleResponse> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new ChangeUserRoleResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Validar que el nuevo rol es válido
            if (!ValidRoles.Contains(request.NewRole))
            {
                return new ChangeUserRoleResponse
                {
                    Success = false,
                    Message = $"Rol inválido. Roles permitidos: {string.Join(", ", ValidRoles)}"
                };
            }

            // Obtener el usuario
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new ChangeUserRoleResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // No permitir cambiar el propio rol
            if (user.UserId == request.RequestingUserId)
            {
                return new ChangeUserRoleResponse
                {
                    Success = false,
                    Message = "No puedes cambiar tu propio rol"
                };
            }

            // Verificar si ya tiene ese rol
            if (user.UserType == request.NewRole)
            {
                return new ChangeUserRoleResponse
                {
                    Success = false,
                    Message = $"El usuario ya tiene el rol {request.NewRole}"
                };
            }

            // Cambiar rol
            user.UserType = request.NewRole;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complete();

            return new ChangeUserRoleResponse
            {
                Success = true,
                Message = $"Rol cambiado exitosamente a {request.NewRole}"
            };
        }
        catch (Exception ex)
        {
            return new ChangeUserRoleResponse
            {
                Success = false,
                Message = $"Error al cambiar rol: {ex.Message}"
            };
        }
    }
}
