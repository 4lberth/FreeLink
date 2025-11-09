using MediatR;
using System.Collections.Generic;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<IEnumerable<AdminUserDto>> 
{ 
    // Esta clase puede estar vacía
}