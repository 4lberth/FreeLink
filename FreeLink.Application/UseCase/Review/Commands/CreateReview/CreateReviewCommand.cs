using MediatR;

namespace FreeLink.Application.UseCase.Review.Commands.CreateReview;

public class CreateReviewCommand : IRequest<CreateReviewResponse>
{
    public int ReviewerId { get; set; }
    public int ReviewedUserId { get; set; }
    public int? ProjectId { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
    public int RequestingUserId { get; set; }
}
