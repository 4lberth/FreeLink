using FreeLink.Application.UseCase.User.Commands.RegisterUser;
using FreeLink.Application.UseCase.User.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var response = await _mediator.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { 
                success = false, 
                message = response.Message 
            });
        }
        
        return Ok(new { 
            success = true, 
            message = response.Message,
            data = new {
                userId = response.UserId,
                email = response.Email,
                userType = response.UserType
            }
        });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserQuery query)
    {
        var response = await _mediator.Send(query);
        
        if (!response.Success)
        {
            return Unauthorized(new { 
                success = false, 
                message = response.Message 
            });
        }
        
        return Ok(new { 
            success = true, 
            message = response.Message,
            token = response.Token,
            user = response.User
        });
    }
}