using FreeLink.Application.UseCase.Review.DTOs;

namespace FreeLink.Application.UseCase.Review.Queries.GetUserReviews;

public class GetUserReviewsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ReviewDto> Reviews { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double AverageRating { get; set; }
}
