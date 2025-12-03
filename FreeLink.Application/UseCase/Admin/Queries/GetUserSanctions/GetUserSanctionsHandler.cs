using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetUserSanctions;

public class GetUserSanctionsHandler : IRequestHandler<GetUserSanctionsQuery, GetUserSanctionsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserSanctionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserSanctionsResponse> Handle(GetUserSanctionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetUserSanctionsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver sanciones"
                };
            }

            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new GetUserSanctionsResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener sanciones del usuario
            var allSanctions = await _unitOfWork.Repository<Usersanction>().GetAll();
            var userSanctions = allSanctions
                .Where(s => s.UserId == request.UserId)
                .OrderByDescending(s => s.StartDate)
                .ToList();

            // Obtener usuarios que aplicaron las sanciones
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear a DTOs
            var sanctionDtos = userSanctions.Select(s =>
            {
                var appliedBy = usersList.FirstOrDefault(u => u.UserId == s.AppliedBy);
                return new SanctionDto
                {
                    SanctionId = s.SanctionId,
                    SanctionType = s.SanctionType,
                    Reason = s.Reason,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive,
                    AppliedBy = s.AppliedBy,
                    AppliedByName = appliedBy?.Email ?? "Administrador no encontrado"
                };
            }).ToList();

            return new GetUserSanctionsResponse
            {
                Success = true,
                Message = "Sanciones obtenidas exitosamente",
                Sanctions = sanctionDtos
            };
        }
        catch (Exception ex)
        {
            return new GetUserSanctionsResponse
            {
                Success = false,
                Message = $"Error al obtener sanciones: {ex.Message}"
            };
        }
    }
}
