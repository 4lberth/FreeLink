using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<GetAllUsersResponse>
{
    // Parámetros opcionales para filtrado y paginación
    public string? UserType { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}