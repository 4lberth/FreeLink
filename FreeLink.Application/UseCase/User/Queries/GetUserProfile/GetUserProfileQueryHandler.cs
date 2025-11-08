using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, GetUserProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.UserId);

            if (user == null)
            {
                return new GetUserProfileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Buscar perfil básico
            var userProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(up => up.UserId == request.UserId);

            // 3. Buscar wallet
            var wallet = await _unitOfWork.Repository<Userwallet>()
                .GetFirstOrDefaultAsync(w => w.UserId == request.UserId);

            // 4. Si es freelancer, buscar su perfil profesional
            FreelancerProfileDto? freelancerProfileDto = null;
            if (user.UserType == "Freelancer")
            {
                var freelancerProfile = await _unitOfWork.Repository<Freelancerprofile>()
                    .GetFirstOrDefaultAsync(fp => fp.UserId == request.UserId);

                if (freelancerProfile != null)
                {
                    freelancerProfileDto = new FreelancerProfileDto
                    {
                        AvailabilityStatus = freelancerProfile.AvailabilityStatus,
                        HourlyRate = freelancerProfile.HourlyRate ,
                        TotalEarnings = freelancerProfile.TotalEarnings ?? 0,
                        CompletedProjects = freelancerProfile.CompletedProjects ?? 0,
                        AverageRating = freelancerProfile.AverageRating ?? 0,
                        TotalReviews = freelancerProfile.TotalReviews ?? 0
                    };
                }
            }

            // 5. Construir el DTO completo
            var profileDto = new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                UserType = user.UserType,
                IsActive = user.IsActive ?? false,
                IsVerified = user.IsVerified ?? false,
                
                FirstName = userProfile?.FirstName ?? string.Empty,
                LastName = userProfile?.LastName ?? string.Empty,
                PhoneNumber = userProfile?.PhoneNumber,
                Country = userProfile?.Country,
                City = userProfile?.City,
                Bio = userProfile?.Bio,
                ProfilePictureUrl = userProfile?.ProfilePicture,
                
                Balance = wallet?.Balance ?? 0,
                PendingBalance = wallet?.PendingBalance ?? 0,
                
                FreelancerProfile = freelancerProfileDto
            };

            // 6. Retornar respuesta exitosa
            return new GetUserProfileResponse
            {
                Success = true,
                Message = "Perfil obtenido exitosamente",
                Profile = profileDto
            };
        }
        catch (Exception ex)
        {
            return new GetUserProfileResponse
            {
                Success = false,
                Message = $"Error al obtener perfil: {ex.Message}"
            };
        }
    }
}