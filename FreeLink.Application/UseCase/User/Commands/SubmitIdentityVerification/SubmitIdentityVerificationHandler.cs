using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.SubmitIdentityVerification;

public class SubmitIdentityVerificationHandler : IRequestHandler<SubmitIdentityVerificationCommand, SubmitIdentityVerificationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;

    public SubmitIdentityVerificationHandler(
        IUnitOfWork unitOfWork,
        IFileService fileService,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _notificationService = notificationService;
    }

    public async Task<SubmitIdentityVerificationResponse> Handle(SubmitIdentityVerificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Validar que no tenga una verificación pendiente o aprobada
            var existingVerifications = await _unitOfWork.Repository<Identityverification>()
                .GetAsync(v => v.UserId == request.UserId);

            var pendingOrApproved = existingVerifications.FirstOrDefault(v =>
                v.VerificationStatus == "Pendiente" || v.VerificationStatus == "Aprobado");

            if (pendingOrApproved != null)
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = pendingOrApproved.VerificationStatus == "Aprobado"
                        ? "Ya tienes una verificación aprobada"
                        : "Ya tienes una solicitud de verificación pendiente"
                };
            }

            // 3. Validar archivos obligatorios
            if (request.DocumentFront == null || request.DocumentBack == null || request.Selfie == null)
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = "Debes proporcionar foto del documento (frente y reverso) y una selfie"
                };
            }

            // 4. Validar tipo y número de documento
            if (string.IsNullOrWhiteSpace(request.DocumentType) || string.IsNullOrWhiteSpace(request.DocumentNumber))
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = "Debes proporcionar tipo y número de documento"
                };
            }

            // 5. Validar formatos de archivo (solo imágenes)
            string[] allowedExtensions = { "jpg", "jpeg", "png" };
            long maxFileSize = 5 * 1024 * 1024; // 5MB

            if (!_fileService.ValidateFile(request.DocumentFront, allowedExtensions, maxFileSize))
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = "La imagen del frente del documento debe ser JPG o PNG y no superar 5MB"
                };
            }

            if (!_fileService.ValidateFile(request.DocumentBack, allowedExtensions, maxFileSize))
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = "La imagen del reverso del documento debe ser JPG o PNG y no superar 5MB"
                };
            }

            if (!_fileService.ValidateFile(request.Selfie, allowedExtensions, maxFileSize))
            {
                return new SubmitIdentityVerificationResponse
                {
                    Success = false,
                    Message = "La selfie debe ser JPG o PNG y no superar 5MB"
                };
            }

            // 6. Subir archivos a Supabase Storage (bucket privado identity-documents)
            var folder = $"user_{request.UserId}";
            var documentFrontUrl = await _fileService.SaveFileAsync(request.DocumentFront, folder, "identity-documents");
            var documentBackUrl = await _fileService.SaveFileAsync(request.DocumentBack, folder, "identity-documents");
            var selfieUrl = await _fileService.SaveFileAsync(request.Selfie, folder, "identity-documents");

            // 7. Crear registro de verificación
            var verification = new Identityverification
            {
                UserId = request.UserId,
                DocumentType = request.DocumentType,
                DocumentNumber = request.DocumentNumber,
                DocumentFrontUrl = documentFrontUrl,
                DocumentBackUrl = documentBackUrl,
                SelfieUrl = selfieUrl,
                VerificationStatus = "Pendiente",
                SubmittedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Identityverification>().Add(verification);
            await _unitOfWork.Complete();

            // 8. Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                userId: request.UserId,
                type: "VerificationSubmitted",
                title: "Solicitud de Verificación Enviada",
                message: "Tu solicitud de verificación de identidad ha sido enviada y está siendo revisada por nuestro equipo",
                resourceType: "Verification",
                resourceId: verification.VerificationId
            );

            // 9. Notificar a administradores
            var admins = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetAsync(u => u.UserType == "Administrador");
            var adminIds = admins.Select(a => a.UserId).ToList();

            if (adminIds.Any())
            {
                await _notificationService.CreateMultipleNotificationsAsync(
                    userIds: adminIds,
                    type: "NewVerificationPending",
                    title: "Nueva Solicitud de Verificación",
                    message: $"El usuario {user.Email} ha enviado una solicitud de verificación de identidad",
                    resourceType: "Verification",
                    resourceId: verification.VerificationId
                );
            }

            return new SubmitIdentityVerificationResponse
            {
                Success = true,
                Message = "Solicitud de verificación enviada exitosamente. Recibirás una notificación cuando sea revisada",
                VerificationId = verification.VerificationId,
                Status = "Pendiente"
            };
        }
        catch (Exception ex)
        {
            return new SubmitIdentityVerificationResponse
            {
                Success = false,
                Message = $"Error al enviar solicitud de verificación: {ex.Message}"
            };
        }
    }
}
