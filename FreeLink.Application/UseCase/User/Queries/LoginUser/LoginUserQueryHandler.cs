using AutoMapper;
using FreeLink.Application.UseCase.User.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.LoginUser;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginUserQueryHandler(
        IUnitOfWork unitOfWork, 
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<LoginUserResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario por email
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetFirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null)
            {
                return new LoginUserResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Verificar si el usuario está activo (conversión explícita de bool? a bool)
            if (user.IsActive != true)  // ← CORREGIDO
            {
                return new LoginUserResponse
                {
                    Success = false,
                    Message = "Usuario inactivo. Contacte al administrador"
                };
            }

            // 3. Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginUserResponse
                {
                    Success = false,
                    Message = "Contraseña incorrecta"
                };
            }

            // 4. Actualizar LastLoginAt
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Update(user);
            await _unitOfWork.Complete();

            // 5. Generar token JWT
            var token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.UserType);

            // 6. Mapear usuario a DTO
            var userDto = _mapper.Map<UserDto>(user);

            // 7. Retornar respuesta exitosa
            return new LoginUserResponse
            {
                Success = true,
                Message = "Login exitoso",
                Token = token,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            return new LoginUserResponse
            {
                Success = false,
                Message = $"Error al iniciar sesión: {ex.Message}"
            };
        }
    }
}