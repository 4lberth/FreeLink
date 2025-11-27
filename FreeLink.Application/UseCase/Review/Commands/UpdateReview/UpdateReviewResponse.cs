namespace FreeLink.Application.UseCase.Review.Commands.UpdateReview;

public class UpdateReviewResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public double? NewAverageRating { get; set; }
}
