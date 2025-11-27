namespace FreeLink.Application.UseCase.Review.Commands.RespondToReview;

public class RespondToReviewResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ResponseId { get; set; }
}
