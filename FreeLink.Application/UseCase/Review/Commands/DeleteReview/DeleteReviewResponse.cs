namespace FreeLink.Application.UseCase.Review.Commands.DeleteReview;

public class DeleteReviewResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public double? NewAverageRating { get; set; }
}
