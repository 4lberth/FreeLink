using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Supportticket
{
    public int TicketId { get; set; }

    public int UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? TicketStatus { get; set; }

    public string? Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? AssignedTo { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual ICollection<Ticketresponse> Ticketresponses { get; set; } = new List<Ticketresponse>();

    public virtual User User { get; set; } = null!;
}
