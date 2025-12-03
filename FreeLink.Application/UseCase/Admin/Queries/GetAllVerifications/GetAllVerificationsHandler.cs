using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllVerifications;

public class GetAllVerificationsHandler : IRequestHandler<GetAllVerificationsQuery, GetAllVerificationsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllVerificationsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllVerificationsResponse> Handle(GetAllVerificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetAllVerificationsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver las verificaciones"
                };
            }

            // Obtener todas las verificaciones
            var verificationsQuery = await _unitOfWork.Repository<Identityverification>().GetAll();
            var verifications = verificationsQuery.ToList();

            // Filtrar por estado si se especificó
            if (!string.IsNullOrWhiteSpace(request.StatusFilter))
            {
                verifications = verifications.Where(v => v.VerificationStatus == request.StatusFilter).ToList();
            }

            // Ordenar por fecha de envío (más recientes primero)
            verifications = verifications.OrderByDescending(v => v.SubmittedAt).ToList();

            // Obtener información de usuarios
            var userIds = verifications.Select(v => v.UserId).Distinct().ToList();
            var reviewerIds = verifications.Where(v => v.ReviewedBy.HasValue).Select(v => v.ReviewedBy!.Value).Distinct().ToList();
            var allUserIds = userIds.Union(reviewerIds).ToList();

            var usersQuery = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetAsync(u => allUserIds.Contains(u.UserId));
            var users = usersQuery.ToList();

            // Mapear a DTOs
            var verificationDtos = verifications.Select(v =>
            {
                var user = users.FirstOrDefault(u => u.UserId == v.UserId);
                var reviewer = v.ReviewedBy.HasValue
                    ? users.FirstOrDefault(u => u.UserId == v.ReviewedBy.Value)
                    : null;

                return new VerificationDto
                {
                    VerificationId = v.VerificationId,
                    UserId = v.UserId,
                    UserEmail = user?.Email ?? "Usuario desconocido",
                    DocumentType = v.DocumentType ?? string.Empty,
                    DocumentNumber = v.DocumentNumber ?? string.Empty,
                    DocumentFrontUrl = v.DocumentFrontUrl ?? string.Empty,
                    DocumentBackUrl = v.DocumentBackUrl ?? string.Empty,
                    SelfieUrl = v.SelfieUrl ?? string.Empty,
                    VerificationStatus = v.VerificationStatus ?? "Pendiente",
                    SubmittedAt = v.SubmittedAt,
                    ReviewedAt = v.ReviewedAt,
                    ReviewedBy = v.ReviewedBy,
                    ReviewedByEmail = reviewer?.Email,
                    RejectionReason = v.RejectionReason
                };
            }).ToList();

            return new GetAllVerificationsResponse
            {
                Success = true,
                Message = $"Se encontraron {verificationDtos.Count} verificaciones",
                Verifications = verificationDtos
            };
        }
        catch (Exception ex)
        {
            return new GetAllVerificationsResponse
            {
                Success = false,
                Message = $"Error al obtener verificaciones: {ex.Message}"
            };
        }
    }
}
