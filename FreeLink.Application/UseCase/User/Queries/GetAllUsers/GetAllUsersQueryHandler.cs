using AutoMapper;
using FreeLink.Application.UseCase.User.DTOs;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener todos los usuarios
            var allUsers = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            
            // 2. Aplicar filtros si existen
            var filteredUsers = allUsers.AsQueryable();

            if (!string.IsNullOrEmpty(request.UserType))
            {
                filteredUsers = filteredUsers.Where(u => u.UserType == request.UserType);
            }

            if (request.IsActive.HasValue)
            {
                filteredUsers = filteredUsers.Where(u => u.IsActive == request.IsActive.Value);
            }

            // 3. Contar total de usuarios después de filtrar
            var totalUsers = filteredUsers.Count();

            // 4. Aplicar paginación
            var paginatedUsers = filteredUsers
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // 5. Mapear a DTOs
            var userDtos = _mapper.Map<List<UserDto>>(paginatedUsers);

            // 6. Calcular total de páginas
            var totalPages = (int)Math.Ceiling(totalUsers / (double)request.PageSize);

            // 7. Retornar respuesta exitosa
            return new GetAllUsersResponse
            {
                Success = true,
                Message = "Usuarios obtenidos exitosamente",
                Users = userDtos,
                TotalUsers = totalUsers,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetAllUsersResponse
            {
                Success = false,
                Message = $"Error al obtener usuarios: {ex.Message}"
            };
        }
    }
}