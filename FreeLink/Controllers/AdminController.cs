using MediatR; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // Aquí pondremos los endpoints
    }
}