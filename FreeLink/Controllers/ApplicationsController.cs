using FreeLink.Application.UseCase.Projects.Commands.CreateApplication;
using FreeLink.Application.UseCase.Projects.Commands.AcceptApplication;
using FreeLink.Application.UseCase.Projects.Commands.RejectApplication;
using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Application.UseCase.Projects.Queries.GetProjectApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreeLink.Controllers;

[ApiController]
[Route("api")]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// Postularse a un proyecto (solo freelancers)
    [HttpPost("projects/{projectId}/applications")]
    [Authorize(Policy = "Freelancer")]
    public async Task<IActionResult> CreateApplication(int projectId, [FromBody] CreateApplicationRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new CreateApplicationCommand
        {
            ProjectId = projectId,
            FreelancerId = userId,
            CoverLetter = request.CoverLetter,
            ProposedRate = request.ProposedRate,
            EstimatedDuration = request.EstimatedDuration,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Ver todas las aplicaciones de un proyecto (solo cliente dueño)
    [HttpGet("projects/{projectId}/applications")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> GetProjectApplications(int projectId)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var query = new GetProjectApplicationsQuery
        {
            ProjectId = projectId,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Aceptar una aplicación (solo cliente dueño del proyecto)
    [HttpPost("applications/{id}/accept")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> AcceptApplication(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new AcceptApplicationCommand
        {
            ApplicationId = id,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Rechazar una aplicación (solo cliente dueño del proyecto)
    [HttpPost("applications/{id}/reject")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> RejectApplication(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new RejectApplicationCommand
        {
            ApplicationId = id,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}