using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetSystemSetting;

public class GetSystemSettingHandler : IRequestHandler<GetSystemSettingQuery, GetSystemSettingResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemSettingHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetSystemSettingResponse> Handle(GetSystemSettingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetSystemSettingResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver configuraciones del sistema"
                };
            }

            // Buscar la configuración
            var allSettings = await _unitOfWork.Repository<Systemsetting>().GetAll();
            var setting = allSettings.FirstOrDefault(s => s.SettingKey == request.SettingKey);

            if (setting == null)
            {
                return new GetSystemSettingResponse
                {
                    Success = false,
                    Message = "Configuración no encontrada"
                };
            }

            // Obtener usuario que actualizó
            FreeLink.Domain.Entities.User? updatedBy = null;
            if (setting.UpdatedBy.HasValue)
            {
                updatedBy = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(setting.UpdatedBy.Value);
            }

            var settingDetails = new SystemSettingDetailsDto
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                Description = setting.Description,
                UpdatedAt = setting.UpdatedAt,
                UpdatedBy = setting.UpdatedBy,
                UpdatedByName = updatedBy?.Email
            };

            return new GetSystemSettingResponse
            {
                Success = true,
                Message = "Configuración obtenida exitosamente",
                Data = settingDetails
            };
        }
        catch (Exception ex)
        {
            return new GetSystemSettingResponse
            {
                Success = false,
                Message = $"Error al obtener configuración: {ex.Message}"
            };
        }
    }
}
