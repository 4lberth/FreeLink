using MediatR; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreeLink.Application.UseCase.Admin.Queries.GetAllUsers;
using FreeLink.Application.UseCase.Admin.Commands.SetUserActiveStatus;
using System.Threading.Tasks;

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
            // Crea y envía la "Query"
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);
            return Ok(users);
        }

        // 👇 MÉTODO 2: AÑADE ESTE ENDPOINT PARA ACTIVAR/DESACTIVAR
        [HttpPost("users/set-active")]
        public async Task<IActionResult> SetUserActiveStatus([FromBody] SetUserActiveStatusCommand command)
        {
            // Envía el "Command"
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound("No se pudo encontrar al usuario.");
            }
            
            return Ok(new { success = true });
        }
    }
}