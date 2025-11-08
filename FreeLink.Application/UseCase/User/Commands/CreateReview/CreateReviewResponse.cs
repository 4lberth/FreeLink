namespace FreeLink.Application.UseCase.User.Commands.CreateReview;

public class CreateReviewResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ReviewId { get; set; }
    public decimal? NewAverageRating { get; set; }
}

