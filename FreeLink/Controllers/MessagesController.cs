using System.Security.Claims;
using FreeLink.Application.UseCase.Messages.Commands.SendMessage;
using FreeLink.Application.UseCase.Messages.Queries.GetProjectMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/projects/{projectId}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendMessage(
        int projectId,
        [FromForm] string content,
        [FromForm] List<IFormFile>? attachments)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new SendMessageCommand
        {
            ProjectId = projectId,
            SenderId = int.Parse(userIdClaim),
            Content = content,
            Attachments = attachments
        };

        var response = await _mediator.Send(command);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(int projectId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetProjectMessagesQuery
        {
            ProjectId = projectId,
            RequestingUserId = int.Parse(userIdClaim)
        };

        var response = await _mediator.Send(query);

        return response.Success ? Ok(response) : BadRequest(response);
    }
}