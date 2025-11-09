using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FreeLink.Application.Services;
using FreeLink.Application.UseCase.Project.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

        public class DeliverableReviewRequest
        {
            public string Decision { get; set; } = string.Empty; // approve | reject | review
            public string? Comments { get; set; }
        }

        [HttpPut("deliverables/{deliverableId}/review")]
        [Authorize]
        public async Task<IActionResult> ReviewDeliverable(int deliverableId, [FromBody] DeliverableReviewRequest request)
        {
            var userIdStr = User.Claims.FirstOrDefault(c =>
                c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            try
            {
                var d = await svc.ReviewDeliverableAsync(deliverableId, userId, request.Decision, request.Comments);
                if (d == null) return NotFound();
                return Ok(d);
            }
            catch (System.UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{projectId}/deliverables")]
        [Authorize]
        public async Task<IActionResult> GetDeliverables(int projectId)
        {
            var list = await svc.GetDeliverablesAsync(projectId);
            return Ok(list);
        }

        [HttpGet("{projectId}/activity")]
        [Authorize]
        public async Task<IActionResult> GetActivity(int projectId)
        {
            var list = await svc.GetActivityAsync(projectId);
            return Ok(list);
        }

        [HttpGet("{projectId}/deliverables/summary")]
        [Authorize]
        public async Task<IActionResult> GetDeliverablesSummary(int projectId)
        {
            var summary = await svc.GetDeliverablesSummaryAsync(projectId);
            return Ok(summary);
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

        // ======= MENSAJERÍA =======
        public class MessagePostRequest
        {
            public string MessageText { get; set; } = string.Empty;
            public IFormFile[]? Files { get; set; }
        }

        [HttpPost("{projectId}/messages")]
        [Authorize]
        public async Task<IActionResult> SendMessage(int projectId, [FromForm] MessagePostRequest request)
        {
            var userIdStr = User.Claims.FirstOrDefault(c =>
                c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            var uploads = (request.Files ?? System.Array.Empty<IFormFile>())
                .Select(f => new FileUploadRequest
                {
                    Stream = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    Length = f.Length
                })
                .ToList();

            try
            {
                var msg = await svc.SendMessageAsync(projectId, userId, request.MessageText, uploads);
                return Ok(msg);
            }
            catch (System.UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{projectId}/messages")]
        [Authorize]
        public async Task<IActionResult> GetMessages(int projectId)
        {
            var messages = await svc.GetMessagesAsync(projectId);
            return Ok(messages);
        }

        public class DeliverableUploadRequest
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }
            public IFormFile[]? Files { get; set; }
        }

        [HttpPost("{projectId}/deliverables")]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> UploadDeliverable(int projectId, [FromForm] DeliverableUploadRequest request)
        {
            var userIdStr = User.Claims.FirstOrDefault(c =>
                c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            var uploads = (request.Files ?? System.Array.Empty<IFormFile>())
                .Select(f => new FileUploadRequest
                {
                    Stream = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    Length = f.Length
                })
                .ToList();

            var dto = new ProjectDeliverableCreateDto
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate.HasValue ? DateOnly.FromDateTime(request.DueDate.Value) : null
            };

            try
            {
                var d = await svc.UploadDeliverableAsync(projectId, userId, dto, uploads);
                if (d == null) return NotFound();
                return Ok(d);
            }
            catch (System.UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}