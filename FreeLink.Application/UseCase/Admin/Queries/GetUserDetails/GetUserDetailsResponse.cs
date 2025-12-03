namespace FreeLink.Application.UseCase.Admin.Queries.GetUserDetails;

public class GetUserDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDetailsDto? UserDetails { get; set; }
}

public class UserDetailsDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
    public bool? IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Stats
    public int TotalProjects { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public int PortfolioItemsCount { get; set; }
    public int WorkExperiencesCount { get; set; }
}
