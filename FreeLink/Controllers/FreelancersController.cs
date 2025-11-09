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
}