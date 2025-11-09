using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FreeLink.Application.Services;
using FreeLink.Application.UseCase.Project.DTOs;

namespace FreeLink.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(IProjectManagementService svc) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectCreateDto dto)
        {
            try
            {
                var project = await svc.CreateProjectAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = project?.ProjectId }, project);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await svc.GetProjectAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            try
            {
                var updated = await svc.UpdateProjectAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await svc.DeleteProjectAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? skills, [FromQuery] string? q)
        {
            var list = await svc.SearchProjectsAsync(skills, q);
            return Ok(list);
        }
        
        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
           
            var success = await svc.StartProjectAsync(id); 
            return success ? Ok(new { message = "Proyecto iniciado." }) : BadRequest("No se puede iniciar el proyecto en su estado actual.");
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
           
            var success = await svc.CompleteProjectAsync(id); 
            return success ? Ok(new { message = "Proyecto completado." }) : BadRequest("No se puede completar el proyecto.");
        }
    }
}