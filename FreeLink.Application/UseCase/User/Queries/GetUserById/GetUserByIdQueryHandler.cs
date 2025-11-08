using AutoMapper;
using FreeLink.Application.UseCase.User.DTOs;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario por ID
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.UserId);

            if (user == null)
            {
                return new GetUserByIdResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Mapear a DTO
            var userDto = _mapper.Map<UserDto>(user);

            // 3. Retornar respuesta exitosa
            return new GetUserByIdResponse
            {
                Success = true,
                Message = "Usuario encontrado",
                User = userDto
            };
        }
        catch (Exception ex)
        {
            return new GetUserByIdResponse
            {
                Success = false,
                Message = $"Error al obtener usuario: {ex.Message}"
            };
        }
    }
}