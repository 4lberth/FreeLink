using System;
using System.Collections.Generic;
using FreeLink.Domain.Enums;

namespace FreeLink.Domain.Entities; 

public partial class Identityverification
{
    public int VerificationId { get; set; }
    
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

    public int UserId { get; set; }

    public string? DocumentType { get; set; }

    public string? DocumentNumber { get; set; }

    public string? DocumentFrontUrl { get; set; }

    public string? DocumentBackUrl { get; set; }

    public string? SelfieUrl { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int? ReviewedBy { get; set; }

    public string? RejectionReason { get; set; }

    public virtual User? ReviewedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
