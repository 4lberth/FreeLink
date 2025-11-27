using System.Security.Claims;
using FreeLink.Application.UseCase.WorkExperience.Commands.AddWorkExperience;
using FreeLink.Application.UseCase.WorkExperience.Commands.DeleteWorkExperience;
using FreeLink.Application.UseCase.WorkExperience.Commands.UpdateWorkExperience;
using FreeLink.Application.UseCase.WorkExperience.DTOs;
using FreeLink.Application.UseCase.WorkExperience.Queries.GetUserWorkExperiences;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/users/{userId}/work-experience")]
public class WorkExperienceController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkExperienceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener todas las experiencias laborales de un usuario
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUserWorkExperiences(int userId)
    {
        var query = new GetUserWorkExperiencesQuery { UserId = userId };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Agregar nueva experiencia laboral
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddWorkExperience(int userId, [FromBody] CreateWorkExperienceRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new AddWorkExperienceCommand
        {
            UserId = userId,
            JobTitle = request.JobTitle,
            Company = request.Company,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent,
            Description = request.Description,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Actualizar experiencia laboral existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateWorkExperience(int userId, int id, [FromBody] UpdateWorkExperienceRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdateWorkExperienceCommand
        {
            ExperienceId = id,
            UserId = userId,
            JobTitle = request.JobTitle,
            Company = request.Company,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent,
            Description = request.Description,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Eliminar experiencia laboral
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteWorkExperience(int userId, int id)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeleteWorkExperienceCommand
        {
            ExperienceId = id,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
