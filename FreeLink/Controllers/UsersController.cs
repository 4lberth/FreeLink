using System.Security.Claims;
using FreeLink.Application.UseCase.User.Commands.ChangePassword;
using FreeLink.Application.UseCase.User.Commands.SubmitIdentityVerification;
using FreeLink.Application.UseCase.User.Commands.UpdateUser;
using FreeLink.Application.UseCase.User.Commands.UpdateUserProfile;
using FreeLink.Application.UseCase.User.Commands.UploadProfilePicture;
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
    
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
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

        var command = new ChangePasswordCommand
        {
            UserId = id,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword,
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

    /// Solicitar verificación de identidad - Sube documentos a Supabase bucket privado
    [HttpPost("{id}/verification")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SubmitIdentityVerification(
        int id,
        [FromForm] SubmitIdentityVerificationRequest request)
    {
        // Obtener datos del usuario autenticado desde el token
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        // Verificar que el usuario solo pueda solicitar su propia verificación
        if (int.Parse(requestingUserId) != id)
        {
            return Forbid();
        }

        var command = new SubmitIdentityVerificationCommand
        {
            UserId = id,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            DocumentFront = request.DocumentFront,
            DocumentBack = request.DocumentBack,
            Selfie = request.Selfie
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Subir foto de perfil - Sube imagen a Supabase bucket público "profiles"
    [HttpPost("{id}/profile-picture")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfilePicture(
        int id,
        [FromForm] UploadProfilePictureRequest request)
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

        // Verificar que el usuario solo pueda subir su propia foto, EXCEPTO si es Administrador
        if (requestingUserRole != "Administrador" && int.Parse(requestingUserId) != id)
        {
            return Forbid();
        }

        var command = new UploadProfilePictureCommand
        {
            UserId = id,
            File = request.File
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}