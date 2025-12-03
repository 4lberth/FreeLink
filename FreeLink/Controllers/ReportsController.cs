using FreeLink.Application.UseCase.Reports.Commands.CreateReport;
using FreeLink.Application.UseCase.Reports.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Cualquier usuario autenticado puede crear reportes
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
    {
        // Obtener el ID del usuario autenticado
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new CreateReportCommand
        {
            ReporterId = userId,
            ReportedUserId = request.ReportedUserId,
            ReportedProjectId = request.ReportedProjectId,
            ReportedMessageId = request.ReportedMessageId,
            ReportReason = request.ReportReason,
            ReportDescription = request.ReportDescription
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}