using System.Security.Claims;
using FreeLink.Application.Contracts;
using FreeLink.Application.UseCase.Contracts.Commands.SignContract;
using FreeLink.Application.UseCase.Contracts.Queries.GetContractByProject;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/contracts")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfService _pdfService;

    public ContractsController(IMediator mediator, IUnitOfWork unitOfWork, IPdfService pdfService)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _pdfService = pdfService;
    }

    /// Obtener contrato del proyecto
    [HttpGet("projects/{projectId}")]
    public async Task<IActionResult> GetContractByProject(int projectId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        var query = new GetContractByProjectQuery
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

    /// Firmar contrato
    [HttpPost("{id}/sign")]
    public async Task<IActionResult> SignContract(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        // Get IP address
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new SignContractCommand
        {
            ContractId = id,
            UserId = int.Parse(userIdClaim),
            IpAddress = ipAddress
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Descargar PDF del contrato (retorna URL de Supabase Storage)
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadContractPdf(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        // 1. Obtener el contrato
        var contract = await _unitOfWork.Repository<Contract>().GetById(id);
        if (contract == null)
        {
            return NotFound(new { success = false, message = "Contrato no encontrado" });
        }

        // 2. Validar que el usuario tiene permiso (cliente o freelancer del contrato)
        int userId = int.Parse(userIdClaim);
        if (contract.ClientId != userId && contract.FreelancerId != userId)
        {
            return Forbid();
        }

        // 3. Verificar si existe la URL del PDF
        if (string.IsNullOrEmpty(contract.ContractPdfUrl))
        {
            // Si no existe, intentar generarlo ahora
            try
            {
                var pdfUrl = await _pdfService.GenerateContractPdfAsync(id);
                contract.ContractPdfUrl = pdfUrl;
                await _unitOfWork.Repository<Contract>().Update(contract);
                await _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error generando PDF: {ex.Message}" });
            }
        }

        // 4. Retornar URL pública de Supabase
        return Ok(new 
        { 
            success = true, 
            pdfUrl = contract.ContractPdfUrl,
            message = "PDF disponible para descarga"
        });
    }
}