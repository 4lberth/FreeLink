using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.RespondToReview;

public class RespondToReviewCommand : IRequest<RespondToReviewResponse>
{
    public int ReviewId { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public int RequestingUserId { get; set; }
}
