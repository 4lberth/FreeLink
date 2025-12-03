using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.UpdateSystemSetting;

public class UpdateSystemSettingHandler : IRequestHandler<UpdateSystemSettingCommand, UpdateSystemSettingResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdminActivityLogger _activityLogger;

    public UpdateSystemSettingHandler(
        IUnitOfWork unitOfWork,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _activityLogger = activityLogger;
    }

    public async Task<UpdateSystemSettingResponse> Handle(UpdateSystemSettingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new UpdateSystemSettingResponse
                {
                    Success = false,
                    Message = "No tienes permisos para actualizar configuraciones del sistema"
                };
            }

            // Validar campos
            if (string.IsNullOrWhiteSpace(request.SettingKey) || string.IsNullOrWhiteSpace(request.SettingValue))
            {
                return new UpdateSystemSettingResponse
                {
                    Success = false,
                    Message = "Debe proporcionar una clave y valor de configuración"
                };
            }

            // Buscar la configuración existente
            var allSettings = await _unitOfWork.Repository<Systemsetting>().GetAll();
            var setting = allSettings.FirstOrDefault(s => s.SettingKey == request.SettingKey);

            if (setting == null)
            {
                return new UpdateSystemSettingResponse
                {
                    Success = false,
                    Message = "Configuración no encontrada"
                };
            }

            var oldValue = setting.SettingValue;

            // Actualizar la configuración
            setting.SettingValue = request.SettingValue;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedBy = request.RequestingAdminId;

            await _unitOfWork.Repository<Systemsetting>().Update(setting);

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "UpdateSystemSetting",
                $"Actualizó configuración '{request.SettingKey}' de '{oldValue}' a '{request.SettingValue}'"
            );

            await _unitOfWork.Complete();

            return new UpdateSystemSettingResponse
            {
                Success = true,
                Message = "Configuración actualizada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateSystemSettingResponse
            {
                Success = false,
                Message = $"Error al actualizar configuración: {ex.Message}"
            };
        }
    }
}
