using System.Security.Claims;
using FreeLink.Application.UseCase.Portfolio.Commands.CreatePortfolioItem;
using FreeLink.Application.UseCase.Portfolio.Commands.DeletePortfolioFile;
using FreeLink.Application.UseCase.Portfolio.Commands.DeletePortfolioItem;
using FreeLink.Application.UseCase.Portfolio.Commands.UpdatePortfolioItem;
using FreeLink.Application.UseCase.Portfolio.Commands.UploadPortfolioFile;
using FreeLink.Application.UseCase.Portfolio.DTOs;
using FreeLink.Application.UseCase.Portfolio.Queries.GetPortfolioItemById;
using FreeLink.Application.UseCase.Portfolio.Queries.GetUserPortfolio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/portfolio")]
public class PortfolioController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortfolioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener portafolio completo de un usuario
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPortfolio(int userId)
    {
        var query = new GetUserPortfolioQuery { UserId = userId };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtener item específico de portafolio
    /// </summary>
    [HttpGet("{portfolioId}")]
    public async Task<IActionResult> GetPortfolioItemById(int portfolioId)
    {
        var query = new GetPortfolioItemByIdQuery { PortfolioId = portfolioId };
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Crear nuevo item en el portafolio
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePortfolioItem([FromBody] CreatePortfolioItemRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new CreatePortfolioItemCommand
        {
            UserId = int.Parse(requestingUserId),
            Title = request.Title,
            Description = request.Description,
            ProjectUrl = request.ProjectUrl,
            CompletionDate = request.CompletionDate,
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
    /// Actualizar item de portafolio
    /// </summary>
    [HttpPut("{portfolioId}")]
    [Authorize]
    public async Task<IActionResult> UpdatePortfolioItem(int portfolioId, [FromBody] UpdatePortfolioItemRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdatePortfolioItemCommand
        {
            PortfolioId = portfolioId,
            Title = request.Title,
            Description = request.Description,
            ProjectUrl = request.ProjectUrl,
            CompletionDate = request.CompletionDate,
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
    /// Eliminar item de portafolio (y todos sus archivos)
    /// </summary>
    [HttpDelete("{portfolioId}")]
    [Authorize]
    public async Task<IActionResult> DeletePortfolioItem(int portfolioId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeletePortfolioItemCommand
        {
            PortfolioId = portfolioId,
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
    /// Subir archivo a un item de portafolio
    /// </summary>
    [HttpPost("{portfolioId}/files")]
    [Consumes("multipart/form-data")]
    [Authorize]
    public async Task<IActionResult> UploadFile(int portfolioId, [FromForm] UploadFileRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UploadPortfolioFileCommand
        {
            PortfolioId = portfolioId,
            File = request.File,
            FileType = request.FileType,
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
    /// Eliminar archivo específico del portafolio
    /// </summary>
    [HttpDelete("files/{fileId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeletePortfolioFileCommand
        {
            FileId = fileId,
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
