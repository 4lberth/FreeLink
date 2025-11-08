using FreeLink.Application.UseCase.User.DTOs;

namespace FreeLink.Application.UseCase.User.Queries.GetAllUsers;

public class GetAllUsersResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<UserDto> Users { get; set; } = new();
}

