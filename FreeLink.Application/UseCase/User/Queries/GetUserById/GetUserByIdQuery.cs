using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
{
    public int UserId { get; set; }
}