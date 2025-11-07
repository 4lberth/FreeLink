using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Deliverablefile
{
    public int FileId { get; set; }

    public int DeliverableId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Projectdeliverable Deliverable { get; set; } = null!;
}
