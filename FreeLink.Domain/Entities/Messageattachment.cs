using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Messageattachment
{
    public int AttachmentId { get; set; }

    public int MessageId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Projectmessage Message { get; set; } = null!;
}
