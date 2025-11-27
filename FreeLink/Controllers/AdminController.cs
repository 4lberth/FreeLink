using System.Security.Claims;
using FreeLink.Application.UseCase.Admin.Commands.ActivateUser;
using FreeLink.Application.UseCase.Admin.Commands.ChangeUserRole;
using FreeLink.Application.UseCase.Admin.Commands.DeactivateUser;
using FreeLink.Application.UseCase.Admin.Commands.DeleteUser;
using FreeLink.Application.UseCase.Admin.Queries.GetUserDetails;
using FreeLink.Application.UseCase.Admin.Queries.SearchUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Buscar usuarios con filtros
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? search,
        [FromQuery] string? userType,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new SearchUsersQuery
        {
            SearchTerm = search,
            UserType = userType,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtener detalles completos de un usuario
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetUserDetailsQuery
        {
            UserId = userId,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Activar un usuario
    /// </summary>
    [HttpPost("users/{userId}/activate")]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ActivateUserCommand
        {
            UserId = userId,
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
    /// Desactivar un usuario
    /// </summary>
    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeactivateUserCommand
        {
            UserId = userId,
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
    /// Cambiar el rol de un usuario
    /// </summary>
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(int userId, [FromBody] ChangeRoleRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ChangeUserRoleCommand
        {
            UserId = userId,
            NewRole = request.NewRole,
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
    /// Eliminar un usuario (soft delete)
    /// </summary>
    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeleteUserCommand
        {
            UserId = userId,
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

public class ChangeRoleRequest
{
    public string NewRole { get; set; } = string.Empty;
}
