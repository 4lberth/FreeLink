using MediatR;
using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.Deliverables.Commands.UploadDeliverable;

public class UploadDeliverableCommand : IRequest<UploadDeliverableResponse>
{
    public int ProjectId { get; set; }
    public int FreelancerId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public List<IFormFile> Files { get; set; } = null!;
}
