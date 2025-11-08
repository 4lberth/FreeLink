using FreeLink.Application.UseCase.User.DTOs;

namespace FreeLink.Application.UseCase.User.Queries.GetAllUsers;

public class GetAllUsersResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<UserDto> Users { get; set; } = new();
    public int TotalUsers { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}