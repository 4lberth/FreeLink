using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectApplications;

public class GetProjectApplicationsHandler : IRequestHandler<GetProjectApplicationsQuery, GetProjectApplicationsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectApplicationsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectApplicationsResponse> Handle(GetProjectApplicationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectApplicationsResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // Solo el cliente dueño puede ver las aplicaciones
            if (project.ClientId != request.RequestingUserId)
            {
                return new GetProjectApplicationsResponse
                {
                    Success = false,
                    Message = "No tienes permiso para ver las aplicaciones de este proyecto"
                };
            }

            var applications = await _unitOfWork.Repository<Projectapplication>()
                .GetAsync(pa => pa.ProjectId == request.ProjectId);

            var applicationDtos = new List<ApplicationDto>();

            foreach (var app in applications.OrderByDescending(a => a.AppliedAt))
            {
                var freelancerProfile = await _unitOfWork.Repository<Userprofile>()
                    .GetFirstOrDefaultAsync(up => up.UserId == app.FreelancerId);

                applicationDtos.Add(new ApplicationDto
                {
                    ApplicationId = app.ApplicationId,
                    ProjectId = app.ProjectId,
                    ProjectTitle = project.Title,
                    FreelancerId = app.FreelancerId,
                    FreelancerName = freelancerProfile != null
                        ? $"{freelancerProfile.FirstName} {freelancerProfile.LastName}"
                        : "Freelancer",
                    CoverLetter = app.CoverLetter,
                    ProposedRate = app.ProposedRate,
                    EstimatedDuration = app.EstimatedDuration,
                    ApplicationStatus = app.ApplicationStatus ?? "Pendiente",
                    AppliedAt = app.AppliedAt,
                    RespondedAt = app.RespondedAt
                });
            }

            return new GetProjectApplicationsResponse
            {
                Success = true,
                Message = $"{applicationDtos.Count} aplicaciones encontradas",
                Applications = applicationDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProjectApplicationsResponse
            {
                Success = false,
                Message = $"Error al obtener aplicaciones: {ex.Message}"
            };
        }
    }
}
