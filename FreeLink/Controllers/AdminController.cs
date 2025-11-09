using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FreeLink.Application.UseCase.Admin.Queries.GetAllUsers;
using FreeLink.Application.UseCase.Admin.Commands.SetUserActiveStatus;
using System.Threading.Tasks; 
using FreeLink.Application.UseCase.Admin.Commands.ChangeUserType;

namespace FreeLink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator; 
    
        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }
    
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);
            return Ok(users);
        }

        [HttpPost("users/set-active")]
        public async Task<IActionResult> SetUserActiveStatus([FromBody] SetUserActiveStatusCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound("No se pudo encontrar al usuario.");
            }
            
            return Ok(new { success = true });
        }

        // 👇 AÑADE ESTE NUEVO ENDPOINT PARA CAMBIAR EL TIPO DE USUARIO
        [HttpPost("users/change-type")]
        public async Task<IActionResult> ChangeUserType([FromBody] ChangeUserTypeCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound("No se pudo encontrar al usuario.");
            }

            return Ok(new { success = true });
        }
    }
}