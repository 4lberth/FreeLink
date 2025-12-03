using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllSanctions;

public class GetAllSanctionsHandler : IRequestHandler<GetAllSanctionsQuery, GetAllSanctionsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSanctionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllSanctionsResponse> Handle(GetAllSanctionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetAllSanctionsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver sanciones"
                };
            }

            // Obtener todas las sanciones
            var allSanctions = await _unitOfWork.Repository<Usersanction>().GetAll();
            var sanctions = allSanctions.AsEnumerable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.SanctionType))
            {
                sanctions = sanctions.Where(s => s.SanctionType == request.SanctionType);
            }

            if (request.IsActive.HasValue)
            {
                sanctions = sanctions.Where(s => s.IsActive == request.IsActive.Value);
            }

            sanctions = sanctions.OrderByDescending(s => s.StartDate);

            var totalCount = sanctions.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedSanctions = sanctions
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener usuarios relacionados
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear a DTOs
            var sanctionDtos = pagedSanctions.Select(s =>
            {
                var user = usersList.FirstOrDefault(u => u.UserId == s.UserId);
                var appliedBy = usersList.FirstOrDefault(u => u.UserId == s.AppliedBy);

                return new SanctionListDto
                {
                    SanctionId = s.SanctionId,
                    UserId = s.UserId,
                    UserName = user?.Email ?? "Usuario no encontrado",
                    UserEmail = user?.Email ?? "",
                    SanctionType = s.SanctionType,
                    Reason = s.Reason,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive,
                    AppliedBy = s.AppliedBy,
                    AppliedByName = appliedBy?.Email ?? "Administrador no encontrado"
                };
            }).ToList();

            return new GetAllSanctionsResponse
            {
                Success = true,
                Message = "Sanciones obtenidas exitosamente",
                Sanctions = sanctionDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetAllSanctionsResponse
            {
                Success = false,
                Message = $"Error al obtener sanciones: {ex.Message}"
            };
        }
    }
}
