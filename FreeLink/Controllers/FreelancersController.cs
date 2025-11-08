using System.Security.Claims;
using FreeLink.Application.UseCase.User.Commands.AddFreelancerSkill;
using FreeLink.Application.UseCase.User.Commands.AddWorkExperience;
using FreeLink.Application.UseCase.User.Commands.CreateReview;
using FreeLink.Application.UseCase.User.Commands.DeleteWorkExperience;
using FreeLink.Application.UseCase.User.Commands.RemoveFreelancerSkill;
using FreeLink.Application.UseCase.User.Commands.UpdateFreelancerProfile;
using FreeLink.Application.UseCase.User.Commands.UploadPortfolioFile;
using FreeLink.Application.UseCase.User.Queries.GetAllSkills;
using FreeLink.Application.UseCase.User.Queries.GetFreelancerProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FreelancersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FreelancersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}/profile")]
    public async Task<IActionResult> GetFreelancerProfile(int userId)
    {
        var query = new GetFreelancerProfileQuery { UserId = userId };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("{userId}/profile")]
    public async Task<IActionResult> UpdateFreelancerProfile(int userId, [FromBody] UpdateFreelancerProfileCommand command)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        // Solo el usuario mismo o un administrador puede actualizar
        if (requestingUserRole != "Administrador" && requestingUserId != userId.ToString())
        {
            return Forbid();
        }

        command.UserId = userId;
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{userId}/skills")]
    public async Task<IActionResult> AddSkill(int userId, [FromBody] AddFreelancerSkillCommand command)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (requestingUserRole != "Administrador" && requestingUserId != userId.ToString())
        {
            return Forbid();
        }

        command.UserId = userId;
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{userId}/skills/{skillId}")]
    public async Task<IActionResult> RemoveSkill(int userId, int skillId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (requestingUserRole != "Administrador" && requestingUserId != userId.ToString())
        {
            return Forbid();
        }

        var command = new RemoveFreelancerSkillCommand
        {
            UserId = userId,
            SkillId = skillId
        };
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{userId}/work-experience")]
    public async Task<IActionResult> AddWorkExperience(int userId, [FromBody] AddWorkExperienceCommand command)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (requestingUserRole != "Administrador" && requestingUserId != userId.ToString())
        {
            return Forbid();
        }

        command.UserId = userId;
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{userId}/work-experience/{experienceId}")]
    public async Task<IActionResult> DeleteWorkExperience(int userId, int experienceId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (requestingUserRole != "Administrador" && requestingUserId != userId.ToString())
        {
            return Forbid();
        }

        var command = new DeleteWorkExperienceCommand
        {
            UserId = userId,
            ExperienceId = experienceId
        };
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{userId}/portfolio")]
    [RequestSizeLimit(50_000_000)] // 50MB límite
    public async Task<IActionResult> UploadPortfolio(int userId, [FromForm] UploadPortfolioFileCommand command)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (requestingUserRole != "Administrador" && requestingUserId != userId.ToString())
        {
            return Forbid();
        }

        command.UserId = userId;
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("reviews")]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        // Verificar que el revisor es el usuario autenticado
        if (requestingUserId != command.ReviewerId.ToString())
        {
            return Forbid();
        }

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("skills")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSkills([FromQuery] string? category)
    {
        var query = new GetAllSkillsQuery { Category = category };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}

