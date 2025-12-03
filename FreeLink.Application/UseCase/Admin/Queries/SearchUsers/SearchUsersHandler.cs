using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.SearchUsers;

public class SearchUsersHandler : IRequestHandler<SearchUsersQuery, SearchUsersResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchUsersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SearchUsersResponse> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new SearchUsersResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Obtener todos los usuarios y aplicar filtros
            var allUsers = await _unitOfWork.Repository<Domain.Entities.User>().GetAll();
            var query = allUsers.AsEnumerable();

            // Filtrar por término de búsqueda (email)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(u => u.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por tipo de usuario
            if (!string.IsNullOrWhiteSpace(request.UserType))
            {
                query = query.Where(u => u.UserType == request.UserType);
            }

            // Filtrar por estado activo/inactivo
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            var usersList = query.OrderByDescending(u => u.CreatedAt).ToList();

            // Calcular paginación
            var totalCount = usersList.Count;
            var users = usersList
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserSummaryDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    UserType = u.UserType,
                    IsActive = u.IsActive,
                    IsVerified = u.IsVerified,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToList();

            return new SearchUsersResponse
            {
                Success = true,
                Message = "Usuarios obtenidos exitosamente",
                Users = users,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            return new SearchUsersResponse
            {
                Success = false,
                Message = $"Error al buscar usuarios: {ex.Message}"
            };
        }
    }
}
