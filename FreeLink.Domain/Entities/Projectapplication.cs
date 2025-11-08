using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Projectapplication
{
    public int ApplicationId { get; set; }

    public int ProjectId { get; set; }

    public int FreelancerId { get; set; }

    public string? CoverLetter { get; set; }

    public decimal? ProposedRate { get; set; }

    public int? EstimatedDuration { get; set; }

    public string? ApplicationStatus { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? RespondedAt { get; set; }

    public virtual User Freelancer { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
