using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Project
{
    public int ProjectId { get; set; }

    public int ClientId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Budget { get; set; }

    public DateOnly DeadlineDate { get; set; }

    public string? ProjectStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? AssignedFreelancerId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? CompletionDate { get; set; }

    public virtual User? AssignedFreelancer { get; set; }

    public virtual User Client { get; set; } = null!;

    public virtual ICollection<Contentreport> Contentreports { get; set; } = new List<Contentreport>();

    public virtual Contract? Contract { get; set; }

    public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();

    public virtual Escrowaccount? Escrowaccount { get; set; }

    public virtual ICollection<Platformcommission> Platformcommissions { get; set; } = new List<Platformcommission>();

    public virtual ICollection<Projectactivitylog> Projectactivitylogs { get; set; } = new List<Projectactivitylog>();

    public virtual ICollection<Projectapplication> Projectapplications { get; set; } = new List<Projectapplication>();

    public virtual ICollection<Projectdeliverable> Projectdeliverables { get; set; } = new List<Projectdeliverable>();

    public virtual ICollection<Projectmessage> Projectmessages { get; set; } = new List<Projectmessage>();

    public virtual ICollection<Projectskill> Projectskills { get; set; } = new List<Projectskill>();

    public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
