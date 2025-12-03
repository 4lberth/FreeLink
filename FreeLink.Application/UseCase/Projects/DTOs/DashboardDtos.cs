namespace FreeLink.Application.UseCase.Projects.DTOs;

public class ProjectDashboardDto
{
    public ProjectSummaryDto Project { get; set; } = null!;
    public ProjectProgressDto Progress { get; set; } = null!;
    public List<DeliverableSummaryDto> RecentDeliverables { get; set; } = new();
    public List<MessageSummaryDto> RecentMessages { get; set; } = new();
    public List<ActivitySummaryDto> RecentActivity { get; set; } = new();
}

public class ProjectProgressDto
{
    public int TotalDeliverables { get; set; }
    public int DeliverablesAprobados { get; set; }
    public int DeliverablesRechazados { get; set; }
    public int DeliverablesEnRevision { get; set; }
    public int DeliverablesPendientes { get; set; }
    public decimal PercentageComplete { get; set; }
    public int TotalMessages { get; set; }
    public int UnreadMessages { get; set; }
}

public class DeliverableSummaryDto
{
    public int DeliverableId { get; set; }
    public string Title { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class MessageSummaryDto
{
    public int MessageId { get; set; }
    public string SenderName { get; set; } = null!;
    public string ContentPreview { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

public class ActivitySummaryDto
{
    public int ActivityId { get; set; }
    public string UserName { get; set; } = null!;
    public string ActivityType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}