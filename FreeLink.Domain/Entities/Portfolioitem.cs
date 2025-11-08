using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Portfolioitem
{
    public int PortfolioId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ProjectUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Portfoliofile> Portfoliofiles { get; set; } = new List<Portfoliofile>();

    public virtual User User { get; set; } = null!;
}
