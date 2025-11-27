using FreeLink.Application.UseCase.WorkExperience.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Queries.GetUserWorkExperiences;

public class GetUserWorkExperiencesHandler : IRequestHandler<GetUserWorkExperiencesQuery, GetUserWorkExperiencesResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserWorkExperiencesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserWorkExperiencesResponse> Handle(GetUserWorkExperiencesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el usuario existe
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new GetUserWorkExperiencesResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Obtener todas las experiencias del usuario
            var experiences = await _unitOfWork.Repository<Workexperience>()
                .GetAsync(we => we.UserId == request.UserId);

            // Ordenar por fecha (más reciente primero, colocando trabajos actuales al inicio)
            var sortedExperiences = experiences
                .OrderByDescending(e => e.IsCurrent)
                .ThenByDescending(e => e.EndDate ?? DateOnly.MaxValue)
                .ThenByDescending(e => e.StartDate)
                .ToList();

            // Mapear a DTOs
            var experienceDtos = sortedExperiences.Select(e => new WorkExperienceDto
            {
                ExperienceId = e.ExperienceId,
                UserId = e.UserId,
                JobTitle = e.JobTitle,
                Company = e.Company,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent ?? false,
                Description = e.Description
            }).ToList();

            return new GetUserWorkExperiencesResponse
            {
                Success = true,
                Message = "Experiencias obtenidas exitosamente",
                Experiences = experienceDtos
            };
        }
        catch (Exception ex)
        {
            return new GetUserWorkExperiencesResponse
            {
                Success = false,
                Message = $"Error al obtener experiencias: {ex.Message}"
            };
        }
    }
}
