namespace FreeLink.Application.UseCase.Proposals.DTOs;

/// <summary>
/// Request para agregar comentario a propuesta
/// </summary>
public class AddCommentRequest
{
    public string CommentText { get; set; } = string.Empty;
}

/// <summary>
/// DTO de comentario en propuesta
/// </summary>
public class ProposalCommentDto
{
    public int CommentId { get; set; }
    public int ProposalId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // "Cliente" o "Freelancer"
    public string CommentText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
