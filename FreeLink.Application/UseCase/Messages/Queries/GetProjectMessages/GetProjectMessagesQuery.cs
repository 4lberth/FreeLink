using MediatR;

namespace FreeLink.Application.UseCase.Messages.Queries.GetProjectMessages;

public class GetProjectMessagesQuery : IRequest<GetProjectMessagesResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
