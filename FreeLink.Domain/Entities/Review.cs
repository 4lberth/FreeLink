using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Review
{
    public int ReviewId { get; set; }

    public int ProjectId { get; set; }

    public int ReviewerId { get; set; }

    public int ReviewedUserId { get; set; }

    public decimal Rating { get; set; }

    public string? ReviewText { get; set; }

    public string ReviewType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User ReviewedUser { get; set; } = null!;

    public virtual User Reviewer { get; set; } = null!;

    public virtual Reviewresponse? Reviewresponse { get; set; }
}
