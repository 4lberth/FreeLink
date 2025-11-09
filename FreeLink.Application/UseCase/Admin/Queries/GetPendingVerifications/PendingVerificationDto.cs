using System;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;

public class PendingVerificationDto
{
    public int VerificationId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } // Lo sacaremos del User
    public string DocumentType { get; set; }
    public string DocumentUrl { get; set; } // Asumiendo que tienes una URL
    public string Status { get; set; } // "Pending", "Approved", etc.
    public DateTime SubmittedAt { get; set; }
}