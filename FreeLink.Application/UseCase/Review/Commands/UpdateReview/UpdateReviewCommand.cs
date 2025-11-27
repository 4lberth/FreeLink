using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.UpdateReview;

public class UpdateReviewCommand : IRequest<UpdateReviewResponse>
{
    public int ReviewId { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
    public int RequestingUserId { get; set; }
}
