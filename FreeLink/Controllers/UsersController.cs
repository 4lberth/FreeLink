using System.Security.Claims;
using FreeLink.Application.UseCase.User.Commands.UpdateUser;
using FreeLink.Application.UseCase.User.Commands.UpdateUserProfile;
using FreeLink.Application.UseCase.User.DTOs;
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
    
    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] string? userType,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllUsersQuery
        {
            UserType = userType,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c => 
            c.Type == "uid" || c.Type == ClaimTypes.NameIdentifier)?.Value;

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
            c.Type == "uid" || c.Type == ClaimTypes.NameIdentifier)?.Value;

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
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
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

        var command = new UpdateUserCommand
        {
            UserId = id,
            Email = request.Email,
            UserType = request.UserType,
            IsActive = request.IsActive,
            IsVerified = request.IsVerified,
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
    
    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserProfileRequest request)
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

        var command = new UpdateUserProfileCommand
        {
            UserId = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Country = request.Country,
            City = request.City,
            Bio = request.Bio,
            ProfilePictureUrl = request.ProfilePictureUrl,
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