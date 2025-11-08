using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Portfoliofile
{
    public int FileId { get; set; }

    public int PortfolioId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Portfolioitem Portfolio { get; set; } = null!;
}
