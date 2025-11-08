using System.Security.Claims;
using FreeLink.Application.UseCase.User.Commands.DeleteUser;
using FreeLink.Application.UseCase.User.Commands.UpdateUser;
using FreeLink.Application.UseCase.User.Queries.GetAllUsers;
using FreeLink.Application.UseCase.User.Queries.GetUserById;
using FreeLink.Application.UseCase.User.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        // Si NO es Administrador Y NO está pidiendo su propio perfil...
        if (requestingUserRole != "Administrador" && requestingUserId != id.ToString())
        {
            return Forbid(); 
        }
        var query = new GetUserByIdQuery { UserId = id };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
    
    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetUserProfile(int id)
    {
        // 1. Obtener datos del token del solicitante
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (requestingUserRole != "Administrador" && requestingUserId != id.ToString())
        {
            return Forbid(); 
        }
        
        var query = new GetUserProfileQuery { UserId = id };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? userType, [FromQuery] bool? isActive)
    {
        var query = new GetAllUsersQuery
        {
            UserType = userType,
            IsActive = isActive
        };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var requestingUserRole = User.Claims.FirstOrDefault(c => 
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        // Solo el usuario mismo o un administrador puede actualizar
        if (requestingUserRole != "Administrador" && requestingUserId != id.ToString())
        {
            return Forbid();
        }

        command.UserId = id;
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var command = new DeleteUserCommand { UserId = id };
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}