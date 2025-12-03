using MediatR;
using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.Messages.Commands.SendMessage;

public class SendMessageCommand : IRequest<SendMessageResponse>
{
    public int ProjectId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = null!;
    public List<IFormFile>? Attachments { get; set; }
}
