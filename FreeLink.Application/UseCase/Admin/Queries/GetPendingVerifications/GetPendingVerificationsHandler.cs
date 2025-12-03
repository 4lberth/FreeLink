using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;

public class GetPendingVerificationsHandler : IRequestHandler<GetPendingVerificationsQuery, GetPendingVerificationsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingVerificationsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPendingVerificationsResponse> Handle(GetPendingVerificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetPendingVerificationsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver verificaciones"
                };
            }

            // Obtener verificaciones pendientes
            var allVerifications = await _unitOfWork.Repository<Identityverification>().GetAll();
            var pendingVerifications = allVerifications
                .Where(v => v.VerificationStatus == "Pendiente")
                .OrderBy(v => v.SubmittedAt)
                .ToList();

            var totalCount = pendingVerifications.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedVerifications = pendingVerifications
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener usuarios y perfiles
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            var userProfiles = await _unitOfWork.Repository<Userprofile>().GetAll();
            var userProfilesList = userProfiles.ToList();

            // Mapear a DTOs
            var verificationDtos = pagedVerifications.Select(v =>
            {
                var user = usersList.FirstOrDefault(u => u.UserId == v.UserId);
                var userProfile = userProfilesList.FirstOrDefault(p => p.UserId == v.UserId);

                var userName = userProfile != null
                    ? $"{userProfile.FirstName} {userProfile.LastName}"
                    : user?.Email ?? "Usuario no encontrado";

                return new VerificationListDto
                {
                    VerificationId = v.VerificationId,
                    UserId = v.UserId,
                    UserName = userName,
                    UserEmail = user?.Email ?? "",
                    DocumentType = v.DocumentType,
                    DocumentNumber = v.DocumentNumber,
                    VerificationStatus = v.VerificationStatus,
                    SubmittedAt = v.SubmittedAt
                };
            }).ToList();

            return new GetPendingVerificationsResponse
            {
                Success = true,
                Message = "Verificaciones obtenidas exitosamente",
                Verifications = verificationDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetPendingVerificationsResponse
            {
                Success = false,
                Message = $"Error al obtener verificaciones: {ex.Message}"
            };
        }
    }
}
