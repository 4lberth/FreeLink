using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Projectdeliverable
{
    public int DeliverableId { get; set; }

    public int ProjectId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? DeliverableStatus { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewComments { get; set; }

    public DateOnly? DueDate { get; set; }

    public virtual ICollection<Deliverablefile> Deliverablefiles { get; set; } = new List<Deliverablefile>();

    public virtual Project Project { get; set; } = null!;
}
