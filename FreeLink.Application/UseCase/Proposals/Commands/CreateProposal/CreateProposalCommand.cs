using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.CreateProposal;

public class CreateProposalCommand : IRequest<CreateProposalResponse>
{
    public int ProjectId { get; set; }
    public int FreelancerId { get; set; }
    public int RequestingUserId { get; set; }
    public decimal TotalCost { get; set; }
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
