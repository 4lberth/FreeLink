using FreeLink.Application.UseCase.Tickets.Commands.CreateTicket;
using FreeLink.Application.UseCase.Tickets.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Cualquier usuario autenticado puede crear tickets
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        // Obtener el ID del usuario autenticado
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new CreateTicketCommand
        {
            UserId = userId,
            Subject = request.Subject,
            Description = request.Description,
            Priority = request.Priority
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}