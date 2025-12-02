using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.CreateApplication;

public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, CreateApplicationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateApplicationHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CreateApplicationResponse> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el usuario es freelancer
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.FreelancerId);
            if (user == null)
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            if (user.UserType != "Freelancer")
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "Solo los freelancers pueden postularse a proyectos"
                };
            }

            // 2. Validar que el requesting user es el mismo freelancer
            if (request.RequestingUserId != request.FreelancerId)
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "No puedes postularte en nombre de otro freelancer"
                };
            }

            // 3. Verificar que el proyecto existe
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 4. Verificar que el proyecto está en estado "Publicado"
            if (project.ProjectStatus != "Publicado")
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "Solo puedes postularte a proyectos en estado 'Publicado'"
                };
            }

            // 5. Verificar que el freelancer no es el cliente del proyecto
            if (project.ClientId == request.FreelancerId)
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "No puedes postularte a tu propio proyecto"
                };
            }

            // 6. Verificar que no existe una aplicación previa
            var existingApplication = await _unitOfWork.Repository<Projectapplication>()
                .GetFirstOrDefaultAsync(pa => pa.ProjectId == request.ProjectId && pa.FreelancerId == request.FreelancerId);

            if (existingApplication != null)
            {
                return new CreateApplicationResponse
                {
                    Success = false,
                    Message = "Ya te has postulado a este proyecto"
                };
            }

            // 7. Crear la aplicación
            var application = new Projectapplication
            {
                ProjectId = request.ProjectId,
                FreelancerId = request.FreelancerId,
                CoverLetter = request.CoverLetter,
                ProposedRate = request.ProposedRate,
                EstimatedDuration = request.EstimatedDuration,
                ApplicationStatus = "Pendiente",
                AppliedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Projectapplication>().Add(application);
            await _unitOfWork.Complete();

            // Registrar actividad
            await _notificationService.LogProjectActivityAsync(
                projectId: request.ProjectId,
                userId: request.FreelancerId,
                activityType: "ApplicationReceived",
                description: $"Nueva aplicación recibida de freelancer ID {request.FreelancerId}"
            );

            return new CreateApplicationResponse
            {
                Success = true,
                Message = "Aplicación enviada exitosamente",
                ApplicationId = application.ApplicationId
            };
        }
        catch (Exception ex)
        {
            return new CreateApplicationResponse
            {
                Success = false,
                Message = $"Error al crear aplicación: {ex.Message}"
            };
        }
    }
}
