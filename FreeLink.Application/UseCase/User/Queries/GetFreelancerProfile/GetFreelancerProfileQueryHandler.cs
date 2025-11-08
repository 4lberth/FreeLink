using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetFreelancerProfile;

public class GetFreelancerProfileQueryHandler : IRequestHandler<GetFreelancerProfileQuery, GetFreelancerProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFreelancerProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetFreelancerProfileResponse> Handle(GetFreelancerProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el usuario existe y es Freelancer
            var user = await _unitOfWork.Repository<User>().GetById(request.UserId);
            if (user == null || user.UserType != "Freelancer")
            {
                return new GetFreelancerProfileResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado o no es un Freelancer"
                };
            }

            // 2. Obtener perfil de freelancer
            var freelancerProfile = await _unitOfWork.Repository<Freelancerprofile>()
                .GetFirstOrDefaultAsync(fp => fp.UserId == request.UserId);

            if (freelancerProfile == null)
            {
                return new GetFreelancerProfileResponse
                {
                    Success = false,
                    Message = "Perfil de freelancer no encontrado"
                };
            }

            // 3. Obtener habilidades
            var freelancerSkills = await _unitOfWork.Repository<Freelancerskill>()
                .GetAsync(fs => fs.UserId == request.UserId);

            var skillsList = freelancerSkills.ToList();
            var skillsDto = new List<SkillDto>();

            foreach (var fs in skillsList)
            {
                var skill = await _unitOfWork.Repository<Skill>().GetById(fs.SkillId);
                if (skill != null)
                {
                    skillsDto.Add(new SkillDto
                    {
                        SkillId = skill.SkillId,
                        SkillName = skill.SkillName,
                        Category = skill.Category,
                        ProficiencyLevel = fs.ProficiencyLevel
                    });
                }
            }

            // 4. Obtener experiencias laborales
            var workExperiences = await _unitOfWork.Repository<Workexperience>()
                .GetAsync(we => we.UserId == request.UserId);

            var workExperiencesDto = workExperiences.Select(we => new WorkExperienceDto
            {
                ExperienceId = we.ExperienceId,
                JobTitle = we.JobTitle,
                Company = we.Company,
                StartDate = we.StartDate,
                EndDate = we.EndDate,
                IsCurrent = we.IsCurrent,
                Description = we.Description
            }).ToList();

            // 5. Obtener items de portafolio
            var portfolioItems = await _unitOfWork.Repository<Portfolioitem>()
                .GetAsync(pi => pi.UserId == request.UserId);

            var portfolioItemsDto = new List<PortfolioItemDto>();

            foreach (var pi in portfolioItems)
            {
                var portfolioFiles = await _unitOfWork.Repository<Portfoliofile>()
                    .GetAsync(pf => pf.PortfolioId == pi.PortfolioId);

                portfolioItemsDto.Add(new PortfolioItemDto
                {
                    PortfolioId = pi.PortfolioId,
                    Title = pi.Title,
                    Description = pi.Description,
                    ProjectUrl = pi.ProjectUrl,
                    ThumbnailUrl = pi.ThumbnailUrl,
                    CompletionDate = pi.CompletionDate,
                    Files = portfolioFiles.Select(pf => new PortfolioFileDto
                    {
                        FileId = pf.FileId,
                        FileName = pf.FileName,
                        FileUrl = pf.FileUrl,
                        FileType = pf.FileType,
                        FileSize = pf.FileSize
                    }).ToList()
                });
            }

            // 6. Construir respuesta
            var profileDto = new FreelancerProfileDto
            {
                FreelancerProfileId = freelancerProfile.FreelancerProfileId,
                UserId = freelancerProfile.UserId,
                Title = freelancerProfile.Title,
                HourlyRate = freelancerProfile.HourlyRate,
                YearsOfExperience = freelancerProfile.YearsOfExperience,
                AvailabilityStatus = freelancerProfile.AvailabilityStatus,
                AverageRating = freelancerProfile.AverageRating,
                TotalReviews = freelancerProfile.TotalReviews,
                Skills = skillsDto,
                WorkExperiences = workExperiencesDto,
                PortfolioItems = portfolioItemsDto
            };

            return new GetFreelancerProfileResponse
            {
                Success = true,
                Message = "Perfil de freelancer obtenido exitosamente",
                Profile = profileDto
            };
        }
        catch (Exception ex)
        {
            return new GetFreelancerProfileResponse
            {
                Success = false,
                Message = $"Error al obtener perfil de freelancer: {ex.Message}"
            };
        }
    }
}

