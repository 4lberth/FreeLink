using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllVerifications;

public class GetAllVerificationsQuery : IRequest<GetAllVerificationsResponse>
{
    public int RequestingAdminId { get; set; }
    public string? StatusFilter { get; set; } // Opcional: "Pendiente", "Aprobada", "Rechazada"
}
