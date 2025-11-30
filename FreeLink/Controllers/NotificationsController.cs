using System.Security.Claims;
using FreeLink.Application.UseCase.Notifications.Commands.MarkNotificationAsRead;
using FreeLink.Application.UseCase.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// Obtener notificaciones del usuario autenticado
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool? unreadOnly = false)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetUserNotificationsQuery
        {
            UserId = int.Parse(userIdClaim),
            UnreadOnly = unreadOnly ?? false
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Marcar notificación como leída
    [HttpPost("{id}/mark-read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new MarkNotificationAsReadCommand
        {
            NotificationId = id,
            UserId = int.Parse(userIdClaim)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener contador de notificaciones no leídas
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetUserNotificationsQuery
        {
            UserId = int.Parse(userIdClaim),
            UnreadOnly = true
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(new
        {
            success = true,
            unreadCount = response.UnreadCount
        });
    }
}