using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Ticketresponse
{
    public int ResponseId { get; set; }

    public int TicketId { get; set; }

    public int ResponderId { get; set; }

    public string ResponseText { get; set; } = null!;

    public bool? IsStaffResponse { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Responder { get; set; } = null!;

    public virtual Supportticket Ticket { get; set; } = null!;
}
