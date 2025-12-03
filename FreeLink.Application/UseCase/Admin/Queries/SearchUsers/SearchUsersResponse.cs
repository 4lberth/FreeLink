namespace FreeLink.Application.UseCase.Admin.Queries.SearchUsers;

public class SearchUsersResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<UserSummaryDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UserSummaryDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
    public bool? IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
