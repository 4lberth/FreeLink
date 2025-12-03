namespace FreeLink.Application.UseCase.Messages.DTOs;

public class MessageDto
{
    public int MessageId { get; set; }
    public int ProjectId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderRole { get; set; } = null!; // Cliente o Freelancer
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
}

public class AttachmentDto
{
    public int AttachmentId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}
