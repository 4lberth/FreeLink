using System.Security.Claims;
using FreeLink.Application.UseCase.Review.Commands.CreateReview;
using FreeLink.Application.UseCase.Review.Commands.DeleteReview;
using FreeLink.Application.UseCase.Review.Commands.RespondToReview;
using FreeLink.Application.UseCase.Review.Commands.UpdateReview;
using FreeLink.Application.UseCase.Review.DTOs;
using FreeLink.Application.UseCase.Review.Queries.GetUserReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener calificaciones de un usuario
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReviews(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUserReviewsQuery 
        { 
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Crear nueva calificación
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new CreateReviewCommand
        {
            ReviewerId = int.Parse(requestingUserId),
            ReviewedUserId = request.ReviewedUserId,
            ProjectId = request.ProjectId,
            Rating = request.Rating,
            Comment = request.ReviewText,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Actualizar calificación existente
    /// </summary>
    [HttpPut("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdateReviewCommand
        {
            ReviewId = reviewId,
            Rating = request.Rating,
            Comment = request.ReviewText,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Eliminar calificación
    /// </summary>
    [HttpDelete("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeleteReviewCommand
        {
            ReviewId = reviewId,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Responder a una calificación
    /// </summary>
    [HttpPost("{reviewId}/response")]
    [Authorize]
    public async Task<IActionResult> RespondToReview(int reviewId, [FromBody] RespondToReviewRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RespondToReviewCommand
        {
            ReviewId = reviewId,
            ResponseText = request.ResponseText,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
