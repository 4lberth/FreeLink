using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Freelancer.Queries.GetFreelancerPublicProfile;

public class GetFreelancerPublicProfileQueryHandler : IRequestHandler<GetFreelancerPublicProfileQuery, GetFreelancerPublicProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFreelancerPublicProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetFreelancerPublicProfileResponse> Handle(GetFreelancerPublicProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetById(request.FreelancerId);

            if (user == null)
            {
                return new GetFreelancerPublicProfileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Verificar que sea freelancer
            if (user.UserType != "Freelancer")
            {
                return new GetFreelancerPublicProfileResponse
                {
                    Success = false,
                    Message = "Este usuario no es un freelancer"
                };
            }

            // 3. Buscar perfil básico
            var userProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(up => up.UserId == request.FreelancerId);

            if (userProfile == null)
            {
                return new GetFreelancerPublicProfileResponse
                {
                    Success = false,
                    Message = "Perfil no encontrado"
                };
            }

            // 4. Buscar perfil profesional del freelancer
            var freelancerProfile = await _unitOfWork.Repository<Freelancerprofile>()
                .GetFirstOrDefaultAsync(fp => fp.UserId == request.FreelancerId);

            if (freelancerProfile == null)
            {
                return new GetFreelancerPublicProfileResponse
                {
                    Success = false,
                    Message = "Perfil de freelancer no encontrado"
                };
            }

            // 5. Obtener habilidades del freelancer
            var freelancerSkills = await _unitOfWork.Repository<Freelancerskill>()
                .GetAsync(fs => fs.UserId == request.FreelancerId);

            var skillIds = freelancerSkills.Select(fs => fs.SkillId).ToList();
            var skills = new List<SkillDto>();

            if (skillIds.Any())
            {
                var allSkills = await _unitOfWork.Repository<Skill>().GetAll();
                skills = allSkills
                    .Where(s => skillIds.Contains(s.SkillId))
                    .Select(s => new SkillDto
                    {
                        SkillId = s.SkillId,
                        SkillName = s.SkillName ?? string.Empty,
                        Category = s.Category
                    })
                    .ToList();
            }

            // 6. Obtener experiencia laboral
            var workExperience = await _unitOfWork.Repository<Workexperience>()
                .GetAsync(we => we.UserId == request.FreelancerId);

            var experienceDtos = workExperience
                .OrderByDescending(we => we.StartDate)
                .Select(we => new WorkExperienceDto
                {
                    ExperienceId = we.ExperienceId,
                    JobTitle = we.JobTitle ?? string.Empty,
                    CompanyName = we.Company ?? string.Empty,
                    StartDate = we.StartDate.ToDateTime(TimeOnly.MinValue), // ✅ CORREGIDO: DateOnly a DateTime
                    EndDate = we.EndDate?.ToDateTime(TimeOnly.MinValue),    // ✅ CORREGIDO: DateOnly? a DateTime?
                    Description = we.Description,
                    IsCurrentJob = we.EndDate == null // ✅ CORREGIDO: Calcular si es trabajo actual
                })
                .ToList();

            // 7. Obtener portafolio con archivos
            var portfolioItems = await _unitOfWork.Repository<Portfolioitem>()
                .GetAsync(pi => pi.UserId == request.FreelancerId);

            var portfolioDtos = new List<PortfolioItemDto>();

            foreach (var item in portfolioItems.OrderByDescending(pi => pi.CreatedAt))
            {
                // Obtener archivos del portafolio
                var files = await _unitOfWork.Repository<Portfoliofile>()
                    .GetAsync(pf => pf.PortfolioId == item.PortfolioId);

                var fileDtos = files.Select(f => new PortfolioFileDto
                {
                    FileId = f.FileId,
                    FileName = f.FileName ?? string.Empty,
                    FileUrl = f.FileUrl ?? string.Empty,
                    FileType = f.FileType ?? string.Empty
                }).ToList();

                portfolioDtos.Add(new PortfolioItemDto
                {
                    PortfolioId = item.PortfolioId,
                    Title = item.Title ?? string.Empty,
                    Description = item.Description,
                    ProjectUrl = item.ProjectUrl,
                    Technologies = string.Empty, // ✅ CORREGIDO: Campo no existe en la entidad
                    CreatedAt = item.CreatedAt,
                    Files = fileDtos
                });
            }

            // 8. Construir el DTO completo del perfil público
            var profileDto = new FreelancerPublicProfileDto
            {
                UserId = user.UserId,
                FirstName = userProfile.FirstName ?? string.Empty,
                LastName = userProfile.LastName ?? string.Empty,
                Country = userProfile.Country,
                City = userProfile.City,
                Bio = userProfile.Bio,
                ProfilePictureUrl = userProfile.ProfilePicture,
                
                AvailabilityStatus = freelancerProfile.AvailabilityStatus ?? string.Empty,
                HourlyRate = freelancerProfile.HourlyRate,
                CompletedProjects = freelancerProfile.CompletedProjects ?? 0,
                AverageRating = freelancerProfile.AverageRating ?? 0,
                TotalReviews = freelancerProfile.TotalReviews ?? 0,
                
                Skills = skills,
                Experience = experienceDtos,
                Portfolio = portfolioDtos
            };

            // 9. Retornar respuesta exitosa
            return new GetFreelancerPublicProfileResponse
            {
                Success = true,
                Message = "Perfil público obtenido exitosamente",
                Profile = profileDto
            };
        }
        catch (Exception ex)
        {
            return new GetFreelancerPublicProfileResponse
            {
                Success = false,
                Message = $"Error al obtener perfil público: {ex.Message}"
            };
        }
    }
}