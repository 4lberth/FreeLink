using AutoMapper;
using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el email no exista
            var emailExists = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .AnyAsync(u => u.Email == request.Email);
            
            if (emailExists)
            {
                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "El email ya está registrado en el sistema"
                };
            }

            // 2. Validar UserType
            var validUserTypes = new[] { "Cliente", "Freelancer", "Administrador" };
            if (!validUserTypes.Contains(request.UserType))
            {
                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "Tipo de usuario inválido. Debe ser: Cliente, Freelancer o Administrador"
                };
            }

            // 3. Crear el usuario
            var newUser = new FreeLink.Domain.Entities.User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserType = request.UserType,
                IsActive = true,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().Add(newUser);
            
            await _unitOfWork.Complete();


            // 4. Crear el perfil básico del usuario
            var userProfile = new Userprofile
            {
                UserId = newUser.UserId,  
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Country = request.Country,
                City = request.City
            };

            await _unitOfWork.Repository<Userprofile>().Add(userProfile);

            // 5. Si es Freelancer, crear FreelancerProfile
            if (request.UserType == "Freelancer")
            {
                var freelancerProfile = new Freelancerprofile
                {
                    UserId = newUser.UserId,
                    AvailabilityStatus = "Disponible",
                    TotalEarnings = 0,
                    CompletedProjects = 0,
                    AverageRating = 0,
                    TotalReviews = 0
                };

                await _unitOfWork.Repository<Freelancerprofile>().Add(freelancerProfile);
            }

            // 6. Crear Wallet para el usuario
            var userWallet = new Userwallet
            {
                UserId = newUser.UserId,
                Balance = 0,
                PendingBalance = 0,
                TotalEarnings = 0,
                TotalSpent = 0,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Userwallet>().Add(userWallet);

            // 7. Guardar todos los cambios restantes
            await _unitOfWork.Complete();

            // 8. Retornar respuesta exitosa
            return new RegisterUserResponse
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                UserId = newUser.UserId,
                Email = newUser.Email,
                UserType = newUser.UserType
            };
        }
        catch (Exception ex)
        {
            return new RegisterUserResponse
            {
                Success = false,
                Message = $"Error al registrar usuario: {ex.Message}"
            };
        }
    }
}