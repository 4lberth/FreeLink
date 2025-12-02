using System.Security.Claims;
using FreeLink.Application.UseCase.Deliverables.Commands.ApproveDeliverable;
using FreeLink.Application.UseCase.Deliverables.Commands.RejectDeliverable;
using FreeLink.Application.UseCase.Deliverables.Commands.SubmitDeliverable;
using FreeLink.Application.UseCase.Deliverables.Commands.UploadDeliverable;
using FreeLink.Application.UseCase.Deliverables.Queries.GetProjectDeliverables;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Authorize]
public class DeliverablesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliverablesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// Sube un nuevo entregable con archivos
    [HttpPost("api/projects/{projectId}/deliverables")]
    [Consumes("multipart/form-data")]
    [Authorize(Policy = "Freelancer")]
    public async Task<IActionResult> UploadDeliverable(
        int projectId,
        [FromForm] string title,
        [FromForm] string? description,
        [FromForm] List<IFormFile> files)
    {
        var userId = GetUserId();

        var command = new UploadDeliverableCommand
        {
            ProjectId = projectId,
            FreelancerId = userId,
            Title = title,
            Description = description,
            Files = files
        };

        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// Envía un entregable a revisión
    [HttpPut("api/deliverables/{deliverableId}/submit")]
    [Authorize(Policy = "Freelancer")]
    public async Task<IActionResult> SubmitDeliverable(int deliverableId)
    {
        var userId = GetUserId();

        var command = new SubmitDeliverableCommand
        {
            DeliverableId = deliverableId,
            FreelancerId = userId
        };

        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// Aprueba un entregable
    [HttpPut("api/deliverables/{deliverableId}/approve")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> ApproveDeliverable(
        int deliverableId,
        [FromBody] ApproveDeliverableRequest? request)
    {
        var userId = GetUserId();

        var command = new ApproveDeliverableCommand
        {
            DeliverableId = deliverableId,
            ClientId = userId,
            Comments = request?.Comments
        };

        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// Rechaza un entregable con comentarios obligatorios
    [HttpPut("api/deliverables/{deliverableId}/reject")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> RejectDeliverable(
        int deliverableId,
        [FromBody] RejectDeliverableRequest request)
    {
        var userId = GetUserId();

        var command = new RejectDeliverableCommand
        {
            DeliverableId = deliverableId,
            ClientId = userId,
            RejectionComments = request.RejectionComments
        };

        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// Obtiene todos los entregables de un proyecto
    [HttpGet("api/projects/{projectId}/deliverables")]
    public async Task<IActionResult> GetProjectDeliverables(int projectId)
    {
        var userId = GetUserId();

        var query = new GetProjectDeliverablesQuery
        {
            ProjectId = projectId,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(query);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    private int GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("Token inválido");
        }

        return int.Parse(userIdClaim);
    }
}

public class ApproveDeliverableRequest
{
    public string? Comments { get; set; }
}

public class RejectDeliverableRequest
{
    public string RejectionComments { get; set; } = null!;
}