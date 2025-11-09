using MediatR;
using System.Collections.Generic;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;

// Pide la lista de DTOs
public class GetPendingVerificationsQuery : IRequest<IEnumerable<PendingVerificationDto>>
{
}