namespace FreeLink.Application.UseCase.User.Queries.GetUserProfile;

public class GetUserProfileResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserProfileDto? Profile { get; set; }
}

public class UserProfileDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    
    // Datos del perfil básico
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    
    // Datos de wallet
    public decimal Balance { get; set; }
    public decimal PendingBalance { get; set; }
    
    // Datos de freelancer (si aplica)
    public FreelancerProfileDto? FreelancerProfile { get; set; }
}

public class FreelancerProfileDto
{
    public string? AvailabilityStatus { get; set; } = string.Empty;
    public decimal? HourlyRate { get; set; }
    public decimal TotalEarnings { get; set; }
    public int CompletedProjects { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
}