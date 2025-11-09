using System.Security.Claims;
using FreeLink.Application.UseCase.Freelancer.Commands.AddFreelancerSkill;
using FreeLink.Application.UseCase.Freelancer.Commands.UpdateFreelancerProfile;
using FreeLink.Application.UseCase.Freelancer.DTOs;
using FreeLink.Application.UseCase.Freelancer.Queries.GetFreelancerPublicProfile;
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
        // Obtener datos del usuario autenticado desde el token
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
}