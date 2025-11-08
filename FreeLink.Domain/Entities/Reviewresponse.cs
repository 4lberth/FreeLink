using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Reviewresponse
{
    public int ResponseId { get; set; }

    public int ReviewId { get; set; }

    public string ResponseText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Review Review { get; set; } = null!;
}
