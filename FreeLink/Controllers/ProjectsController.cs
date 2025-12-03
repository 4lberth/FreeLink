using FreeLink.Application.UseCase.Projects.Commands.CreateProject;
using FreeLink.Application.UseCase.Projects.Commands.UpdateProject;
using FreeLink.Application.UseCase.Projects.Commands.DeleteProject;
using FreeLink.Application.UseCase.Projects.DTOs;
using FreeLink.Application.UseCase.Projects.Queries.GetProjectById;
using FreeLink.Application.UseCase.Projects.Queries.GetProjectDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// Crear un nuevo proyecto (solo clientes)
    [HttpPost]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new CreateProjectCommand
        {
            ClientId = userId,
            Title = request.Title,
            Description = request.Description,
            Budget = request.Budget,
            DeadlineDate = request.DeadlineDate,
            RequiredSkillIds = request.RequiredSkillIds,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles de un proyecto
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var userId = GetCurrentUserId();

        var query = new GetProjectByIdQuery
        {
            ProjectId = id,
            RequestingUserId = userId > 0 ? userId : null
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Actualizar un proyecto existente (solo cliente dueño)
    [HttpPut("{id}")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new UpdateProjectCommand
        {
            ProjectId = id,
            Title = request.Title,
            Description = request.Description,
            Budget = request.Budget,
            DeadlineDate = request.DeadlineDate,
            RequiredSkillIds = request.RequiredSkillIds,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Cancelar un proyecto (soft delete - solo cliente dueño)
    [HttpDelete("{id}")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new DeleteProjectCommand
        {
            ProjectId = id,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Buscar proyectos públicos (para freelancers)
    [HttpGet]
    public async Task<IActionResult> SearchProjects([FromQuery] int? skillId, [FromQuery] decimal? budgetMin, [FromQuery] decimal? budgetMax)
    {
        var query = new FreeLink.Application.UseCase.Projects.Queries.SearchProjects.SearchProjectsQuery
        {
            SkillId = skillId,
            BudgetMin = budgetMin,
            BudgetMax = budgetMax,
            Status = "Publicado"
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Iniciar un proyecto (cambiar estado a "En Proceso")
    [HttpPost("{id}/start")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> StartProject(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new FreeLink.Application.UseCase.Projects.Commands.StartProject.StartProjectCommand
        {
            ProjectId = id,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Completar un proyecto (cambiar estado a "Completado")
    [HttpPost("{id}/complete")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> CompleteProject(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new FreeLink.Application.UseCase.Projects.Commands.CompleteProject.CompleteProjectCommand
        {
            ProjectId = id,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener todos los proyectos del cliente autenticado (incluyendo cancelados)
    [HttpGet("my-projects")]
    [Authorize]
    public async Task<IActionResult> GetMyProjects([FromQuery] string? status)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var query = new FreeLink.Application.UseCase.Projects.Queries.GetClientProjects.GetClientProjectsQuery
        {
            ClientId = userId,
            StatusFilter = status
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener historial de actividades del proyecto
    [HttpGet("{id}/activity")]
    public async Task<IActionResult> GetProjectActivity(int id)
    {
        var query = new FreeLink.Application.UseCase.ActivityLogs.Queries.GetProjectActivityLog.GetProjectActivityLogQuery
        {
            ProjectId = id
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener dashboard del proyecto con progreso, entregables, mensajes y actividad reciente
    [HttpGet("{id}/dashboard")]
    [Authorize]
    public async Task<IActionResult> GetProjectDashboard(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var query = new GetProjectDashboardQuery
        {
            ProjectId = id,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}