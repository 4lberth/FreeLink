namespace FreeLink.Application.UseCase.Deliverables.DTOs;

public class DeliverableDto
{
    public int DeliverableId { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!; // Pendiente, Enviado, EnRevision, Aprobado, Rechazado
    public DateTime UploadedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComments { get; set; }
    public List<DeliverableFileDto> Files { get; set; } = new();
}

public class DeliverableFileDto
{
    public int FileId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}
