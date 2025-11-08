using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<GetAllUsersResponse>
{
    public string? UserType { get; set; }
    public bool? IsActive { get; set; }
}

