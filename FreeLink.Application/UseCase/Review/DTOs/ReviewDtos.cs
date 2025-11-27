namespace FreeLink.Application.UseCase.Review.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
    public int ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public int ReviewedUserId { get; set; }
    public string? ReviewedUserName { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public string ReviewType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ReviewResponseDto? Response { get; set; }
}

public class ReviewResponseDto
{
    public int ResponseId { get; set; }
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public int ProjectId { get; set; }
    public int ReviewedUserId { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public string ReviewType { get; set; } = string.Empty;
}

public class UpdateReviewRequest
{
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
}

public class RespondToReviewRequest
{
    public string ResponseText { get; set; } = string.Empty;
}
