using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            
            if (user == null)
            {
                return new DeleteUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Soft delete: marcar como inactivo en lugar de eliminar físicamente
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Repository<Domain.Entities.User>().Update(user);
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

