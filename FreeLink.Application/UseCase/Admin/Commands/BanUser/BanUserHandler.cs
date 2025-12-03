using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.BanUser;

public class BanUserHandler : IRequestHandler<BanUserCommand, BanUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public BanUserHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<BanUserResponse> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new BanUserResponse
                {
                    Success = false,
                    Message = "No tienes permisos para banear usuarios"
                };
            }

            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new BanUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Evitar banear a otros administradores
            if (user.UserType == "Administrador")
            {
                return new BanUserResponse
                {
                    Success = false,
                    Message = "No se puede banear a un administrador"
                };
            }

            // Validar razón
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return new BanUserResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una razón para el baneo permanente"
                };
            }

            // Verificar que no tenga un baneo permanente activo
            var existingBans = await _unitOfWork.Repository<Usersanction>()
                .GetAsync(s => s.UserId == request.UserId &&
                              s.SanctionType == "Baneo Permanente" &&
                              s.IsActive == true);

            if (existingBans.Any())
            {
                return new BanUserResponse
                {
                    Success = false,
                    Message = "El usuario ya tiene un baneo permanente activo"
                };
            }

            // Crear la sanción (permanente, sin EndDate)
            var sanction = new Usersanction
            {
                UserId = request.UserId,
                SanctionType = "Baneo Permanente",
                Reason = request.Reason,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                IsActive = true,
                AppliedBy = request.RequestingAdminId
            };

            await _unitOfWork.Repository<Usersanction>().Add(sanction);

            // Desactivar al usuario permanentemente
            user.IsActive = false;
            await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);

            // Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                request.UserId,
                "admin_action",
                "Cuenta Baneada Permanentemente",
                $"Tu cuenta ha sido baneada permanentemente. Razón: {request.Reason}",
                "user",
                request.UserId
            );

            // Registrar actividad del administrador
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "BanUser",
                $"Baneó permanentemente al usuario #{request.UserId}"
            );

            await _unitOfWork.Complete();

            return new BanUserResponse
            {
                Success = true,
                Message = "Usuario baneado permanentemente"
            };
        }
        catch (Exception ex)
        {
            return new BanUserResponse
            {
                Success = false,
                Message = $"Error al banear usuario: {ex.Message}"
            };
        }
    }
}
