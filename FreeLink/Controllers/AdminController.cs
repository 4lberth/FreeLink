using System.Security.Claims;
using FreeLink.Application.UseCase.Admin.Commands.ActivateUser;
using FreeLink.Application.UseCase.Admin.Commands.ApproveVerification;
using FreeLink.Application.UseCase.Admin.Commands.AssignDispute;
using FreeLink.Application.UseCase.Admin.Commands.AssignTicket;
using FreeLink.Application.UseCase.Admin.Commands.BanUser;
using FreeLink.Application.UseCase.Admin.Commands.ChangeUserRole;
using FreeLink.Application.UseCase.Admin.Commands.DeactivateUser;
using FreeLink.Application.UseCase.Admin.Commands.DeleteUser;
using FreeLink.Application.UseCase.Admin.Commands.DismissReport;
using FreeLink.Application.UseCase.Admin.Commands.IssueWarning;
using FreeLink.Application.UseCase.Admin.Commands.RejectVerification;
using FreeLink.Application.UseCase.Admin.Commands.RemoveSanction;
using FreeLink.Application.UseCase.Admin.Commands.ReplyToTicket;
using FreeLink.Application.UseCase.Admin.Commands.ResolveDispute;
using FreeLink.Application.UseCase.Admin.Commands.ResolveReport;
using FreeLink.Application.UseCase.Admin.Commands.SuspendUser;
using FreeLink.Application.UseCase.Admin.Commands.UpdateDisputeStatus;
using FreeLink.Application.UseCase.Admin.Commands.UpdateSystemSetting;
using FreeLink.Application.UseCase.Admin.Commands.UpdateTicketStatus;
using FreeLink.Application.UseCase.Admin.Commands.ApproveWithdrawal;
using FreeLink.Application.UseCase.Admin.Commands.RejectWithdrawal;
using FreeLink.Application.UseCase.Admin.Queries.GetAdminActivityLogs;
using FreeLink.Application.UseCase.Admin.Queries.GetAllSanctions;
using FreeLink.Application.UseCase.Admin.Queries.GetAllTransactions;
using FreeLink.Application.UseCase.Admin.Queries.GetDashboardStats;
using FreeLink.Application.UseCase.Admin.Queries.GetDisputeDetails;
using FreeLink.Application.UseCase.Admin.Queries.GetOpenTickets;
using FreeLink.Application.UseCase.Admin.Queries.GetPendingDisputes;
using FreeLink.Application.UseCase.Admin.Queries.GetAllReports;
using FreeLink.Application.UseCase.Admin.Queries.GetPendingReports;
using FreeLink.Application.UseCase.Admin.Queries.GetAllVerifications;
using FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;
using FreeLink.Application.UseCase.Admin.Queries.GetReportDetails;
using FreeLink.Application.UseCase.Admin.Queries.GetSystemSetting;
using FreeLink.Application.UseCase.Admin.Queries.GetSystemSettings;
using FreeLink.Application.UseCase.Admin.Queries.GetTicketDetails;
using FreeLink.Application.UseCase.Admin.Queries.GetTransactionDetails;
using FreeLink.Application.UseCase.Admin.Queries.GetTransactionStatistics;
using FreeLink.Application.UseCase.Admin.Queries.GetUserActivityLogs;
using FreeLink.Application.UseCase.Admin.Queries.GetPendingWithdrawals;
using FreeLink.Application.UseCase.Admin.Queries.GetUserDetails;
using FreeLink.Application.UseCase.Admin.Queries.GetUserSanctions;
using FreeLink.Application.UseCase.Admin.Queries.GetVerificationDetails;
using FreeLink.Application.UseCase.Admin.Queries.SearchUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLink.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "Administrador")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// Obtener estadísticas generales del dashboard administrativo
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetDashboardStatsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Buscar usuarios con filtros
    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? search,
        [FromQuery] string? userType,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new SearchUsersQuery
        {
            SearchTerm = search,
            UserType = userType,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles completos de un usuario
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetUserDetailsQuery
        {
            UserId = userId,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Activar un usuario
    [HttpPost("users/{userId}/activate")]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ActivateUserCommand
        {
            UserId = userId,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Desactivar un usuario
    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeactivateUserCommand
        {
            UserId = userId,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Cambiar el rol de un usuario
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(int userId, [FromBody] ChangeRoleRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ChangeUserRoleCommand
        {
            UserId = userId,
            NewRole = request.NewRole,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Eliminar un usuario (soft delete)
    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DeleteUserCommand
        {
            UserId = userId,
            RequestingUserId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }


    /// Obtener verificaciones pendientes con paginación
    [HttpGet("verifications/pending")]
    public async Task<IActionResult> GetPendingVerifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetPendingVerificationsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener todas las verificaciones con filtro opcional por estado
    [HttpGet("verifications")]
    public async Task<IActionResult> GetAllVerifications([FromQuery] string? status = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetAllVerificationsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            StatusFilter = status
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles completos de una verificación
    [HttpGet("verifications/{verificationId}")]
    public async Task<IActionResult> GetVerificationDetails(int verificationId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetVerificationDetailsQuery
        {
            VerificationId = verificationId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Aprobar una verificación de identidad
    [HttpPost("verifications/{verificationId}/approve")]
    public async Task<IActionResult> ApproveVerification(int verificationId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ApproveVerificationCommand
        {
            VerificationId = verificationId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Rechazar una verificación de identidad
    [HttpPost("verifications/{verificationId}/reject")]
    public async Task<IActionResult> RejectVerification(int verificationId, [FromBody] RejectVerificationRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RejectVerificationCommand
        {
            VerificationId = verificationId,
            RequestingAdminId = int.Parse(requestingUserId),
            RejectionReason = request.RejectionReason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    // ==================== MODERACIÓN DE CONTENIDO ====================

    /// Obtener reportes pendientes con paginación
    [HttpGet("reports/pending")]
    public async Task<IActionResult> GetPendingReports(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? reportType = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetPendingReportsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize,
            ReportType = reportType
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener todos los reportes con filtros opcionales
    [HttpGet("reports")]
    public async Task<IActionResult> GetAllReports(
        [FromQuery] string? status = null,
        [FromQuery] string? reportType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetAllReportsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            StatusFilter = status,
            ReportType = reportType,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles completos de un reporte
    [HttpGet("reports/{reportId}")]
    public async Task<IActionResult> GetReportDetails(int reportId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetReportDetailsQuery
        {
            ReportId = reportId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Resolver un reporte de contenido
    [HttpPost("reports/{reportId}/resolve")]
    public async Task<IActionResult> ResolveReport(int reportId, [FromBody] ResolveReportRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ResolveReportCommand
        {
            ReportId = reportId,
            RequestingAdminId = int.Parse(requestingUserId),
            Resolution = request.Resolution,
            Action = request.Action
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Desestimar un reporte de contenido
    [HttpPost("reports/{reportId}/dismiss")]
    public async Task<IActionResult> DismissReport(int reportId, [FromBody] DismissReportRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new DismissReportCommand
        {
            ReportId = reportId,
            RequestingAdminId = int.Parse(requestingUserId),
            Reason = request.Reason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    // ==================== SISTEMA DE SANCIONES ====================

    /// Emitir una advertencia a un usuario
    [HttpPost("sanctions/warning")]
    public async Task<IActionResult> IssueWarning([FromBody] IssueWarningRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new IssueWarningCommand
        {
            UserId = request.UserId,
            RequestingAdminId = int.Parse(requestingUserId),
            Reason = request.Reason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Suspender temporalmente a un usuario
    [HttpPost("sanctions/suspend")]
    public async Task<IActionResult> SuspendUser([FromBody] SuspendUserRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new SuspendUserCommand
        {
            UserId = request.UserId,
            RequestingAdminId = int.Parse(requestingUserId),
            Reason = request.Reason,
            DurationDays = request.DurationDays
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Banear permanentemente a un usuario
    [HttpPost("sanctions/ban")]
    public async Task<IActionResult> BanUser([FromBody] BanUserRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new BanUserCommand
        {
            UserId = request.UserId,
            RequestingAdminId = int.Parse(requestingUserId),
            Reason = request.Reason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Remover una sanción
    [HttpDelete("sanctions/{sanctionId}")]
    public async Task<IActionResult> RemoveSanction(int sanctionId, [FromBody] RemoveSanctionRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RemoveSanctionCommand
        {
            SanctionId = sanctionId,
            RequestingAdminId = int.Parse(requestingUserId),
            Reason = request.Reason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener todas las sanciones de un usuario
    [HttpGet("sanctions/user/{userId}")]
    public async Task<IActionResult> GetUserSanctions(int userId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetUserSanctionsQuery
        {
            UserId = userId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Obtener todas las sanciones del sistema con paginación
    [HttpGet("sanctions")]
    public async Task<IActionResult> GetAllSanctions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sanctionType = null,
        [FromQuery] bool? isActive = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetAllSanctionsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize,
            SanctionType = sanctionType,
            IsActive = isActive
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    // ==================== TICKETS DE SOPORTE ====================

    /// Asignar un ticket a un administrador
    [HttpPost("tickets/{ticketId}/assign")]
    public async Task<IActionResult> AssignTicket(int ticketId, [FromBody] AssignTicketRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new AssignTicketCommand
        {
            TicketId = ticketId,
            AdminId = request.AdminId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Actualizar el estado de un ticket
    [HttpPut("tickets/{ticketId}/status")]
    public async Task<IActionResult> UpdateTicketStatus(int ticketId, [FromBody] UpdateTicketStatusRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdateTicketStatusCommand
        {
            TicketId = ticketId,
            RequestingAdminId = int.Parse(requestingUserId),
            NewStatus = request.NewStatus
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Responder a un ticket de soporte
    [HttpPost("tickets/{ticketId}/reply")]
    public async Task<IActionResult> ReplyToTicket(int ticketId, [FromBody] ReplyToTicketRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ReplyToTicketCommand
        {
            TicketId = ticketId,
            RequestingAdminId = int.Parse(requestingUserId),
            ResponseText = request.ResponseText
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener tickets abiertos/en progreso con paginación
    [HttpGet("tickets")]
    public async Task<IActionResult> GetOpenTickets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetOpenTicketsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize,
            Status = status,
            Priority = priority
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles completos de un ticket
    [HttpGet("tickets/{ticketId}")]
    public async Task<IActionResult> GetTicketDetails(int ticketId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetTicketDetailsQuery
        {
            TicketId = ticketId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    // ==================== GESTIÓN DE DISPUTAS ====================

    /// Asignar una disputa a un mediador
    [HttpPost("disputes/{disputeId}/assign")]
    public async Task<IActionResult> AssignDispute(int disputeId, [FromBody] AssignDisputeRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new AssignDisputeCommand
        {
            DisputeId = disputeId,
            MediatorId = request.MediatorId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Actualizar el estado de una disputa
    [HttpPut("disputes/{disputeId}/status")]
    public async Task<IActionResult> UpdateDisputeStatus(int disputeId, [FromBody] UpdateDisputeStatusRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdateDisputeStatusCommand
        {
            DisputeId = disputeId,
            RequestingAdminId = int.Parse(requestingUserId),
            NewStatus = request.NewStatus
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Resolver una disputa
    [HttpPost("disputes/{disputeId}/resolve")]
    public async Task<IActionResult> ResolveDispute(int disputeId, [FromBody] ResolveDisputeRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ResolveDisputeCommand
        {
            DisputeId = disputeId,
            RequestingAdminId = int.Parse(requestingUserId),
            Resolution = request.Resolution,
            WinningPartyId = request.WinningPartyId
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener disputas pendientes/en revisión con paginación
    [HttpGet("disputes")]
    public async Task<IActionResult> GetPendingDisputes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetPendingDisputesQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize,
            Status = status
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles completos de una disputa
    [HttpGet("disputes/{disputeId}")]
    public async Task<IActionResult> GetDisputeDetails(int disputeId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetDisputeDetailsQuery
        {
            DisputeId = disputeId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    // ==================== CONFIGURACIONES DEL SISTEMA ====================

    /// Obtener todas las configuraciones del sistema
    [HttpGet("settings")]
    public async Task<IActionResult> GetSystemSettings()
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetSystemSettingsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener una configuración específica por clave
    [HttpGet("settings/{settingKey}")]
    public async Task<IActionResult> GetSystemSetting(string settingKey)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetSystemSettingQuery
        {
            SettingKey = settingKey,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Actualizar una configuración del sistema
    [HttpPut("settings/{settingKey}")]
    public async Task<IActionResult> UpdateSystemSetting(string settingKey, [FromBody] UpdateSystemSettingRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new UpdateSystemSettingCommand
        {
            SettingKey = settingKey,
            SettingValue = request.SettingValue,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    // ==================== VISTA DE TRANSACCIONES ====================

    /// Obtener todas las transacciones con filtros y paginación
    [HttpGet("transactions")]
    public async Task<IActionResult> GetAllTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? transactionType = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetAllTransactionsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize,
            TransactionType = transactionType,
            Status = status,
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener detalles de una transacción específica
    [HttpGet("transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionDetails(int transactionId)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetTransactionDetailsQuery
        {
            TransactionId = transactionId,
            RequestingAdminId = int.Parse(requestingUserId)
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// Obtener estadísticas de transacciones
    [HttpGet("transactions/statistics")]
    public async Task<IActionResult> GetTransactionStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetTransactionStatisticsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    // ==================== LOGS DE ACTIVIDAD ====================

    /// Obtener logs de actividad de administradores
    [HttpGet("logs/admin")]
    public async Task<IActionResult> GetAdminActivityLogs(
        [FromQuery] int? adminId = null,
        [FromQuery] string? actionType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetAdminActivityLogsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            AdminId = adminId,
            ActionType = actionType,
            StartDate = startDate,
            EndDate = endDate,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Obtener logs de actividad de un usuario específico
    [HttpGet("logs/user/{userId}")]
    public async Task<IActionResult> GetUserActivityLogs(
        int userId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetUserActivityLogsQuery
        {
            UserId = userId,
            RequestingAdminId = int.Parse(requestingUserId),
            StartDate = startDate,
            EndDate = endDate,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    // ======================== WITHDRAWAL MANAGEMENT ========================

    /// Obtener retiros pendientes de aprobación
    [HttpGet("withdrawals/pending")]
    public async Task<IActionResult> GetPendingWithdrawals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var query = new GetPendingWithdrawalsQuery
        {
            RequestingAdminId = int.Parse(requestingUserId),
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Aprobar solicitud de retiro
    [HttpPost("withdrawals/{withdrawalId}/approve")]
    public async Task<IActionResult> ApproveWithdrawal(int withdrawalId, [FromBody] ApproveWithdrawalRequest? request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new ApproveWithdrawalCommand
        {
            WithdrawalId = withdrawalId,
            AdminId = int.Parse(requestingUserId),
            AdminNotes = request?.AdminNotes
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// Rechazar solicitud de retiro
    [HttpPost("withdrawals/{withdrawalId}/reject")]
    public async Task<IActionResult> RejectWithdrawal(int withdrawalId, [FromBody] RejectWithdrawalRequest request)
    {
        var requestingUserId = User.Claims.FirstOrDefault(c =>
            c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(requestingUserId))
        {
            return Unauthorized(new { success = false, message = "Token inválido" });
        }

        var command = new RejectWithdrawalCommand
        {
            WithdrawalId = withdrawalId,
            AdminId = int.Parse(requestingUserId),
            RejectionReason = request.RejectionReason
        };

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}

public class ChangeRoleRequest
{
    public string NewRole { get; set; } = string.Empty;
}

public class RejectVerificationRequest
{
    public string RejectionReason { get; set; } = string.Empty;
}

public class ResolveReportRequest
{
    public string Resolution { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class DismissReportRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class IssueWarningRequest
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class SuspendUserRequest
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int DurationDays { get; set; }
}

public class BanUserRequest
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RemoveSanctionRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class AssignTicketRequest
{
    public int AdminId { get; set; }
}

public class UpdateTicketStatusRequest
{
    public string NewStatus { get; set; } = string.Empty;
}

public class ReplyToTicketRequest
{
    public string ResponseText { get; set; } = string.Empty;
}

public class AssignDisputeRequest
{
    public int MediatorId { get; set; }
}

public class UpdateDisputeStatusRequest
{
    public string NewStatus { get; set; } = string.Empty;
}

public class ResolveDisputeRequest
{
    public string Resolution { get; set; } = string.Empty;
    public int? WinningPartyId { get; set; }
}

public class UpdateSystemSettingRequest
{
    public string SettingValue { get; set; } = string.Empty;
}

public class ApproveWithdrawalRequest
{
    public string? AdminNotes { get; set; }
}

public class RejectWithdrawalRequest
{
    public string RejectionReason { get; set; } = string.Empty;
}