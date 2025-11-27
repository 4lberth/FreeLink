using System.Security.Claims;
using FreeLink.Application.UseCase.Freelancer.Commands.AddFreelancerSkill;
using FreeLink.Application.UseCase.Freelancer.Commands.UpdateFreelancerProfile;
using FreeLink.Application.UseCase.Freelancer.DTOs;
using FreeLink.Application.UseCase.Freelancer.Queries.GetFreelancerPublicProfile;
using FreeLink.Application.UseCase.Skills.Commands.CreateSkill;
using FreeLink.Application.UseCase.Skills.Commands.RemoveFreelancerSkill;
using FreeLink.Application.UseCase.Skills.Queries.GetAllSkills;
using FreeLink.Application.UseCase.Skills.Queries.GetFreelancerSkills;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreelancersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FreelancersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}/profile")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFreelancerPublicProfile(int id)
    {
        var query = new GetFreelancerPublicProfileQuery { FreelancerId = id };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
    
    [HttpPut("{id}/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateFreelancerProfile(int id, [FromBody] UpdateFreelancerProfileRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "userType" || c.Type == ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdateFreelancerProfileCommand
        {
            FreelancerId = id,
            HourlyRate = request.HourlyRate,
            AvailabilityStatus = request.AvailabilityStatus,
            ProfessionalTitle = request.ProfessionalTitle,
            RequestingUserId = int.Parse(requestingUserId),
            RequestingUserRole = requestingUserRole ?? string.Empty
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
    [HttpPost("{id}/skills")]
    [Authorize]
    public async Task<IActionResult> AddFreelancerSkill(int id, [FromBody] AddFreelancerSkillRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "userType" || c.Type == ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new AddFreelancerSkillCommand
        {
            FreelancerId = id,
            SkillId = request.SkillId,
            RequestingUserId = int.Parse(requestingUserId),
            RequestingUserRole = requestingUserRole ?? string.Empty
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
    /// <summary>
    /// Obtener habilidades de un freelancer
    /// </summary>
    [HttpGet("{id}/skills")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFreelancerSkills(int id)
    {
        var query = new GetFreelancerSkillsQuery { FreelancerId = id };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
    
    /// <summary>
    /// Eliminar una habilidad de un freelancer
    /// </summary>
    [HttpDelete("{id}/skills/{skillId}")]
    [Authorize]
    public async Task<IActionResult> RemoveFreelancerSkill(int id, int skillId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RemoveFreelancerSkillCommand
        {
            FreelancerId = id,
            SkillId = skillId,
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

/// <summary>
/// Controller para gestión de habilidades
/// </summary>
[ApiController]
[Route("api/skills")]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener todas las habilidades disponibles
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSkills()
    {
        var query = new GetAllSkillsQuery();
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Crear una nueva habilidad (solo administradores)
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new CreateSkillCommand
        {
            SkillName = request.SkillName,
            Category = request.Category,
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

public class CreateSkillRequest
{
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}