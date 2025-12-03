using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetSystemSettings;

public class GetSystemSettingsHandler : IRequestHandler<GetSystemSettingsQuery, GetSystemSettingsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemSettingsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetSystemSettingsResponse> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetSystemSettingsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver configuraciones del sistema"
                };
            }

            // Obtener todas las configuraciones
            var allSettings = await _unitOfWork.Repository<Systemsetting>().GetAll();
            var settings = allSettings.OrderBy(s => s.SettingKey).ToList();

            // Obtener usuarios para mostrar quién actualizó cada configuración
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear a DTOs
            var settingDtos = settings.Select(s =>
            {
                var updatedBy = s.UpdatedBy.HasValue ? usersList.FirstOrDefault(u => u.UserId == s.UpdatedBy.Value) : null;
                return new SystemSettingDto
                {
                    SettingId = s.SettingId,
                    SettingKey = s.SettingKey,
                    SettingValue = s.SettingValue,
                    Description = s.Description,
                    UpdatedAt = s.UpdatedAt,
                    UpdatedBy = s.UpdatedBy,
                    UpdatedByName = updatedBy?.Email
                };
            }).ToList();

            return new GetSystemSettingsResponse
            {
                Success = true,
                Message = "Configuraciones obtenidas exitosamente",
                Settings = settingDtos
            };
        }
        catch (Exception ex)
        {
            return new GetSystemSettingsResponse
            {
                Success = false,
                Message = $"Error al obtener configuraciones: {ex.Message}"
            };
        }
    }
}
