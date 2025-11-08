using AutoMapper;
using FreeLink.Application.UseCase.User.DTOs;
using FreeLink.Domain.Entities;
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
            IEnumerable<Domain.Entities.User> users;

            // Si hay filtros, usar GetAsync con predicado
            if (!string.IsNullOrEmpty(request.UserType) && request.IsActive.HasValue)
            {
                users = await _unitOfWork.Repository<Domain.Entities.User>()
                    .GetAsync(u => u.UserType == request.UserType && u.IsActive == request.IsActive.Value);
            }
            else if (!string.IsNullOrEmpty(request.UserType))
            {
                users = await _unitOfWork.Repository<Domain.Entities.User>()
                    .GetAsync(u => u.UserType == request.UserType);
            }
            else if (request.IsActive.HasValue)
            {
                users = await _unitOfWork.Repository<Domain.Entities.User>()
                    .GetAsync(u => u.IsActive == request.IsActive.Value);
            }
            else
            {
                users = await _unitOfWork.Repository<Domain.Entities.User>().GetAll();
            }

            var userList = users.ToList();
            var userDtos = _mapper.Map<List<UserDto>>(userList);

            return new GetAllUsersResponse
            {
                Success = true,
                Message = "Usuarios obtenidos exitosamente",
                Users = userDtos
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

