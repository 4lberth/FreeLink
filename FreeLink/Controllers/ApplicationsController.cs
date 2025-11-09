using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using FreeLink.Application.Services;
using FreeLink.Application.UseCase.Application.DTOs;

namespace FreeLink.WebAPI.Controllers
{
    [ApiController]
    [Route("api")] // Usamos ruta base para construir URLs estilo RESTful
    public class ApplicationsController(IApplicationService applicationService) : ControllerBase
    {
        [HttpPost("projects/{projectId}/applications")]
        public async Task<IActionResult> Submit(int projectId, [FromBody] ApplicationCreateDto dto)
        {
            try
            {
                var result = await applicationService.SubmitApplicationAsync(projectId, dto);
                return Ok(result); // O CreatedAtAction si prefieres
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("projects/{projectId}/applications")]
        public async Task<ActionResult<IEnumerable<ApplicationViewDto>>> GetByProject(int projectId)
        {
            return Ok(await applicationService.GetApplicationsForProjectAsync(projectId));
        }

        [HttpGet("freelancers/{freelancerId}/applications")]
        public async Task<ActionResult<IEnumerable<ApplicationViewDto>>> GetByFreelancer(int freelancerId)
        {
            return Ok(await applicationService.GetApplicationsByFreelancerAsync(freelancerId));
        }
        [HttpPost("applications/{id}/accept")]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                var success = await applicationService.AcceptApplicationAsync(id);
                return success ? Ok(new { message = "Postulación aceptada." }) : NotFound();
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("applications/{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await applicationService.RejectApplicationAsync(id);
            return success ? Ok(new { message = "Postulación rechazada." }) : NotFound();
        }
    }
}