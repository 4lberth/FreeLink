using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetUserDetails;

public class GetUserDetailsHandler : IRequestHandler<GetUserDetailsQuery, GetUserDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserDetailsResponse> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario solicitante es admin
            var requestingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.RequestingUserId);
            if (requestingUser == null || requestingUser.UserType != "Administrador")
            {
                return new GetUserDetailsResponse
                {
                    Success = false,
                    Message = "No tienes permisos de administrador"
                };
            }

            // Obtener el usuario
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new GetUserDetailsResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener estadísticas del usuario
            var projects = await _unitOfWork.Repository<Project>().GetAsync(p => 
                p.ClientId == request.UserId || p.AssignedFreelancerId == request.UserId);
            
            var reviews = await _unitOfWork.Repository<Domain.Entities.Review>().GetAsync(r => 
                r.ReviewedUserId == request.UserId);
            
            var portfolioItems = await _unitOfWork.Repository<Portfolioitem>().GetAsync(p => 
                p.UserId == request.UserId);
            
            var workExperiences = await _unitOfWork.Repository<Workexperience>().GetAsync(w => 
                w.UserId == request.UserId);

            var averageRating = reviews.Any() 
                ? Math.Round((double)reviews.Average(r => r.Rating), 2) 
                : 0;

            var userDetails = new UserDetailsDto
            {
                UserId = user.UserId,
                Email = user.Email,
                UserType = user.UserType,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                TotalProjects = projects.Count(),
                TotalReviews = reviews.Count(),
                AverageRating = averageRating,
                PortfolioItemsCount = portfolioItems.Count(),
                WorkExperiencesCount = workExperiences.Count()
            };

            return new GetUserDetailsResponse
            {
                Success = true,
                Message = "Detalles del usuario obtenidos exitosamente",
                UserDetails = userDetails
            };
        }
        catch (Exception ex)
        {
            return new GetUserDetailsResponse
            {
                Success = false,
                Message = $"Error al obtener detalles: {ex.Message}"
            };
        }
    }
}
