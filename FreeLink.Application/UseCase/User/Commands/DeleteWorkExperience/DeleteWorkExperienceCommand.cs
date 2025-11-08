using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.DeleteWorkExperience;

public class DeleteWorkExperienceCommand : IRequest<DeleteWorkExperienceResponse>
{
    public int ExperienceId { get; set; }
    public int UserId { get; set; } // Para verificar que el usuario es el dueño
}

