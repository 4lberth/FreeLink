using FreeLink.Application.UseCase.Messages.DTOs;

namespace FreeLink.Application.UseCase.Messages.Queries.GetProjectMessages;

public class GetProjectMessagesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<MessageDto> Messages { get; set; } = new();
}
