using FreeLink.Application.UseCase.Payments.Commands.DepositEscrow;
using FreeLink.Application.UseCase.Payments.Commands.ReleasePayment;
using FreeLink.Application.UseCase.Payments.Queries.GetUserTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Depositar fondos en garantía (escrow) para un proyecto
    /// </summary>
    [HttpPost("projects/{projectId}/escrow/deposit")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> DepositEscrow(int projectId, [FromBody] decimal amount)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new DepositEscrowCommand
        {
            ProjectId = projectId,
            ClientId = userId,
            Amount = amount
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Liberar pago al aprobar un entregable (automático)
    /// Este endpoint será llamado automáticamente por el sistema al aprobar un entregable
    /// </summary>
    [HttpPost("deliverables/{deliverableId}/release-payment")]
    [Authorize(Policy = "Cliente")]
    public async Task<IActionResult> ReleasePayment(int deliverableId)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new ReleasePaymentCommand
        {
            DeliverableId = deliverableId,
            ClientId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtener historial de transacciones del usuario con filtros opcionales
    /// </summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetUserTransactions(
        [FromQuery] string? type = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? projectId = null,
        [FromQuery] int limit = 50)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var query = new GetUserTransactionsQuery
        {
            UserId = userId,
            TransactionType = type,
            StartDate = startDate,
            EndDate = endDate,
            ProjectId = projectId,
            Limit = limit
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtener balance del usuario autenticado
    /// </summary>
    [HttpGet("balance")]
    public async Task<IActionResult> GetUserBalance()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var query = new FreeLink.Application.UseCase.Payments.Queries.GetUserBalance.GetUserBalanceQuery
        {
            UserId = userId
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Solicitar retiro de fondos disponibles
    /// </summary>
    [HttpPost("withdraw")]
    [Authorize(Policy = "Freelancer")]
    public async Task<IActionResult> RequestWithdrawal([FromBody] decimal amount)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new FreeLink.Application.UseCase.Payments.Commands.RequestWithdrawal.RequestWithdrawalCommand
        {
            UserId = userId,
            Amount = amount
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Generar y obtener comprobante de pago en PDF
    /// </summary>
    [HttpGet("transactions/{transactionId}/receipt")]
    public async Task<IActionResult> GetPaymentReceipt(int transactionId)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
        {
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        var command = new FreeLink.Application.UseCase.Payments.Commands.GeneratePaymentReceipt.GeneratePaymentReceiptCommand
        {
            TransactionId = transactionId,
            RequestingUserId = userId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        // Devolver el PDF como archivo descargable
        if (response.PdfBytes != null)
        {
            return File(response.PdfBytes, "application/pdf", $"recibo_{transactionId}.pdf");
        }

        return Ok(response);
        
    }
}