using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.CreateApplication;

public class CreateApplicationCommand : IRequest<CreateApplicationResponse>
{
    public int ProjectId { get; set; }
    public int FreelancerId { get; set; }
    public string? CoverLetter { get; set; }
    public decimal? ProposedRate { get; set; }
    public int? EstimatedDuration { get; set; }
    public int RequestingUserId { get; set; }
}
