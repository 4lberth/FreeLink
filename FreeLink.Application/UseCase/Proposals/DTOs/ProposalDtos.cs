namespace FreeLink.Application.UseCase.Proposals.DTOs;

/// <summary>
/// Request para crear una nueva propuesta
/// </summary>
public class CreateProposalRequest
{
    public int ProjectId { get; set; }
    public int FreelancerId { get; set; }
    public decimal TotalCost { get; set; }
    public List<CostBreakdownItemDto> CostBreakdown { get; set; } = new();
    public List<TimelineMilestoneDto> Timeline { get; set; } = new();
    public List<DeliverableItemDto> Deliverables { get; set; } = new();
}

/// <summary>
/// Request para crear nueva versión de propuesta
/// </summary>
public class CreateNewVersionRequest
{
    public int ProjectId { get; set; }
    public int FreelancerId { get; set; }
    public decimal TotalCost { get; set; }
    public List<CostBreakdownItemDto> CostBreakdown { get; set; } = new();
    public List<TimelineMilestoneDto> Timeline { get; set; } = new();
    public List<DeliverableItemDto> Deliverables { get; set; } = new();
    public string? ChangeReason { get; set; } // Razón de los cambios
}

/// <summary>
/// Request para actualizar propuesta existente
/// </summary>
public class UpdateProposalRequest
{
    public decimal TotalCost { get; set; }
    public List<CostBreakdownItemDto> CostBreakdown { get; set; } = new();
    public List<TimelineMilestoneDto> Timeline { get; set; } = new();
    public List<DeliverableItemDto> Deliverables { get; set; } = new();
}

/// <summary>
/// Item de desglose de costos
/// </summary>
public class CostBreakdownItemDto
{
    public string ItemDescription { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int ItemOrder { get; set; }
}

/// <summary>
/// Hito del cronograma
/// </summary>
public class TimelineMilestoneDto
{
    public string MilestoneName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedDuration { get; set; } // En días
    public int ItemOrder { get; set; }
}

/// <summary>
/// Entregable de la propuesta
/// </summary>
public class DeliverableItemDto
{
    public string DeliverableName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ItemOrder { get; set; }
}

/// <summary>
/// DTO completo de propuesta
/// </summary>
public class ProposalDto
{
    public int ProposalId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public int FreelancerId { get; set; }
    public string FreelancerName { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public decimal TotalCost { get; set; }
    public string ProposalStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Detalles de la propuesta
    public List<CostBreakdownItemDto> CostBreakdown { get; set; } = new();
    public List<TimelineMilestoneDto> Timeline { get; set; } = new();
    public List<DeliverableItemDto> Deliverables { get; set; } = new();
}

/// <summary>
/// DTO resumido para listados
/// </summary>
public class ProposalSummaryDto
{
    public int ProposalId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public int FreelancerId { get; set; }
    public string FreelancerName { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public decimal TotalCost { get; set; }
    public string ProposalStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
