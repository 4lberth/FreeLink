using System.Security.Claims;
using FreeLink.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PaymentsController(FreeLinkContext db) : ControllerBase
{
    public sealed record TxDto(
        int TransactionId,
        string TransactionType,
        string TransactionStatus,
        decimal Amount,
        int? FromUserId,
        int? ToUserId,
        int? EscrowId,
        DateTime CreatedAt,
        DateTime? CompletedAt,
        string? ReceiptUrl,
        string? Description
    );

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] string? type,
        [FromQuery] string? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        // usuario y rol desde el JWT
        var userId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var role = User.Claims.FirstOrDefault(c =>
            c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var uid = int.Parse(userId);

        var q = db.Transactions.AsNoTracking().AsQueryable();

        // Si NO es admin, solo ve donde participa
        if (!string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            q = q.Where(t => t.FromUserId == uid || t.ToUserId == uid);
        }

        if (!string.IsNullOrWhiteSpace(type))
            q = q.Where(t => t.TransactionType == type);

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(t => t.TransactionStatus == status);

        if (from.HasValue) q = q.Where(t => t.CreatedAt >= from.Value);
        if (to.HasValue)   q = q.Where(t => t.CreatedAt <= to.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TxDto(
                t.TransactionId,
                t.TransactionType,
                t.TransactionStatus ?? "",
                t.Amount,
                t.FromUserId,
                t.ToUserId,
                t.EscrowId,
                t.CreatedAt,
                t.CompletedAt,
                t.ReceiptUrl,
                t.Description
            ))
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }
}
