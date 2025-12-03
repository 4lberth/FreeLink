using System.Security.Claims;
using FreeLink.Application.UseCase.Proposals.Commands.AcceptProposal;
using FreeLink.Application.UseCase.Proposals.Commands.AddProposalComment;
using FreeLink.Application.UseCase.Proposals.Commands.CreateNewProposalVersion;
using FreeLink.Application.UseCase.Proposals.Commands.CreateProposal;
using FreeLink.Application.UseCase.Proposals.Commands.RejectProposal;
using FreeLink.Application.UseCase.Proposals.Commands.RequestChanges;
using FreeLink.Application.UseCase.Proposals.DTOs;
using FreeLink.Application.UseCase.Proposals.Queries.GetProposalById;
using FreeLink.Application.UseCase.Proposals.Queries.GetProposalComments;
using FreeLink.Application.UseCase.Proposals.Queries.GetProposalsByProject;
using Commands = FreeLink.Application.UseCase.Proposals.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/proposals")]
[Authorize]
public class ProposalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProposalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// Crear nueva propuesta
    [HttpPost]
    [Authorize(Policy = "Freelancer")]
    public async Task<IActionResult> CreateProposal([FromBody] CreateProposalRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new CreateProposalCommand
        {
            ProjectId = request.ProjectId,
            FreelancerId = request.FreelancerId,
            RequestingUserId = int.Parse(userIdClaim),
            TotalCost = request.TotalCost,
            CostBreakdown = request.CostBreakdown.Select(cb => new Commands.CreateProposal.CostBreakdownItem
            {
                ItemDescription = cb.ItemDescription,
                Amount = cb.Amount,
                ItemOrder = cb.ItemOrder
            }).ToList(),
            Timeline = request.Timeline.Select(t => new Commands.CreateProposal.TimelineMilestone
            {
                MilestoneName = t.MilestoneName,
                Description = t.Description,
                EstimatedDuration = t.EstimatedDuration,
                ItemOrder = t.ItemOrder
            }).ToList(),
            Deliverables = request.Deliverables.Select(d => new Commands.CreateProposal.DeliverableItem
            {
                DeliverableName = d.DeliverableName,
                Description = d.Description,
                ItemOrder = d.ItemOrder
            }).ToList()
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener todas las propuestas de un proyecto
    [HttpGet("projects/{projectId}")]
    public async Task<IActionResult> GetProposalsByProject(int projectId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        var query = new GetProposalsByProjectQuery
        {
            ProjectId = projectId,
            RequestingUserId = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener propuesta por ID con todos los detalles
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProposalById(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        var query = new GetProposalByIdQuery
        {
            ProposalId = id,
            RequestingUserId = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Aceptar propuesta (solo cliente)
    [HttpPost("{id}/accept")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> AcceptProposal(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new AcceptProposalCommand
        {
            ProposalId = id,
            RequestingUserId = int.Parse(userIdClaim)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Solicitar cambios en propuesta (solo cliente)
    [HttpPost("{id}/request-changes")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> RequestChanges(int id, [FromBody] RequestChangesRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RequestChangesCommand
        {
            ProposalId = id,
            RequestingUserId = int.Parse(userIdClaim),
            ChangeReason = request.ChangeReason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Rechazar propuesta (solo cliente)
    [HttpPost("{id}/reject")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> RejectProposal(int id, [FromBody] RejectProposalRequest? request = null)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RejectProposalCommand
        {
            ProposalId = id,
            RequestingUserId = int.Parse(userIdClaim),
            RejectionReason = request?.RejectionReason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Agregar comentario a propuesta
    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new AddProposalCommentCommand
        {
            ProposalId = id,
            UserId = int.Parse(userIdClaim),
            CommentText = request.CommentText
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener comentarios de propuesta
    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetComments(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        var query = new GetProposalCommentsQuery
        {
            ProposalId = id,
            RequestingUserId = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Crear nueva versión de propuesta (solo freelancer)
    [HttpPost("projects/{projectId}/new-version")]
    [Authorize(Policy = "Freelancer")]
    public async Task<IActionResult> CreateNewVersion(int projectId, [FromBody] CreateNewVersionRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var userId = int.Parse(userIdClaim);

        var command = new CreateNewProposalVersionCommand
        {
            ProjectId = projectId,
            FreelancerId = userId,
            RequestingUserId = userId,
            TotalCost = request.TotalCost,
            ChangeReason = request.ChangeReason,
            CostBreakdown = request.CostBreakdown.Select(cb => new FreeLink.Application.UseCase.Proposals.Commands.CreateNewProposalVersion.CostBreakdownItem
            {
                ItemDescription = cb.ItemDescription,
                Amount = cb.Amount,
                ItemOrder = cb.ItemOrder
            }).ToList(),
            Timeline = request.Timeline.Select(t => new FreeLink.Application.UseCase.Proposals.Commands.CreateNewProposalVersion.TimelineMilestone
            {
                MilestoneName = t.MilestoneName,
                Description = t.Description,
                EstimatedDuration = t.EstimatedDuration,
                ItemOrder = t.ItemOrder
            }).ToList(),
            Deliverables = request.Deliverables.Select(d => new FreeLink.Application.UseCase.Proposals.Commands.CreateNewProposalVersion.DeliverableItem
            {
                DeliverableName = d.DeliverableName,
                Description = d.Description,
                ItemOrder = d.ItemOrder
            }).ToList()
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}

// Request DTOs for controller endpoints
public class RequestChangesRequest
{
    public string ChangeReason { get; set; } = string.Empty;
}

public class RejectProposalRequest
{
    public string? RejectionReason { get; set; }
}

// Request DTO for creating new version
public class CreateNewVersionRequest
{
    public decimal TotalCost { get; set; }
    public string? ChangeReason { get; set; }
    public List<CostBreakdownItem> CostBreakdown { get; set; } = new();
    public List<TimelineMilestone> Timeline { get; set; } = new();
    public List<DeliverableItem> Deliverables { get; set; } = new();
}

public class CostBreakdownItem
{
    public string ItemDescription { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int ItemOrder { get; set; }
}

public class TimelineMilestone
{
    public string MilestoneName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedDuration { get; set; }
    public int ItemOrder { get; set; }
}

public class DeliverableItem
{
    public string DeliverableName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ItemOrder { get; set; }
}