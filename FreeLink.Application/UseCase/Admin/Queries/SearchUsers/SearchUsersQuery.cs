using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.SearchUsers;

public class SearchUsersQuery : IRequest<SearchUsersResponse>
{
    public string? SearchTerm { get; set; }
    public string? UserType { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int RequestingUserId { get; set; }
}
