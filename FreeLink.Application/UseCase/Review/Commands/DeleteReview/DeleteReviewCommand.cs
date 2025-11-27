using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.DeleteReview;

public class DeleteReviewCommand : IRequest<DeleteReviewResponse>
{
    public int ReviewId { get; set; }
    public int RequestingUserId { get; set; }
}
