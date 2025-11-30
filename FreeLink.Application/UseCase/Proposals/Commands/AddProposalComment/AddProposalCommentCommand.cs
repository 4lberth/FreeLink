using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.AddProposalComment;

public class AddProposalCommentCommand : IRequest<AddProposalCommentResponse>
{
    public int ProposalId { get; set; }
    public int UserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
}
