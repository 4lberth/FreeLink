namespace FreeLink.Application.UseCase.Proposals.Commands.AddProposalComment;

public class AddProposalCommentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CommentId { get; set; }
}
