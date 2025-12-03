namespace FreeLink.Application.UseCase.Messages.Commands.SendMessage;

public class SendMessageResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? MessageId { get; set; }
}
