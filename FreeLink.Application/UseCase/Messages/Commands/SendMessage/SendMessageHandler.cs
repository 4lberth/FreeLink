using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Messages.Commands.SendMessage;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;
    private static readonly string[] AllowedExtensions = { "pdf", "doc", "docx", "jpg", "jpeg", "png", "gif", "zip", "rar", "txt" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public SendMessageHandler(IUnitOfWork unitOfWork, IFileService fileService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _notificationService = notificationService;
    }

    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el contenido no esté vacío
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return new SendMessageResponse
                {
                    Success = false,
                    Message = "El contenido del mensaje no puede estar vacío"
                };
            }

            // 2. Obtener y validar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new SendMessageResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Verificar permisos (solo cliente o freelancer del proyecto pueden enviar mensajes)
            bool isClient = project.ClientId == request.SenderId;
            bool isFreelancer = project.AssignedFreelancerId == request.SenderId;

            if (!isClient && !isFreelancer)
            {
                return new SendMessageResponse
                {
                    Success = false,
                    Message = "No tienes permiso para enviar mensajes en este proyecto"
                };
            }

            // 4. Crear el mensaje
            var message = new Projectmessage
            {
                ProjectId = request.ProjectId,
                SenderId = request.SenderId,
                MessageText = request.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.Repository<Projectmessage>().Add(message);
            await _unitOfWork.Complete();

            // 5. Procesar archivos adjuntos si los hay
            if (request.Attachments != null && request.Attachments.Any())
            {
                foreach (var file in request.Attachments)
                {
                    // Validar archivo
                    if (!_fileService.ValidateFile(file, AllowedExtensions, MaxFileSize))
                    {
                        continue; // Saltar archivos inválidos
                    }

                    // Subir archivo
                    var folder = $"messages/project_{request.ProjectId}";
                    var fileUrl = await _fileService.SaveFileAsync(file, folder);

                    // Crear registro de adjunto
                    var attachment = new Messageattachment
                    {
                        MessageId = message.MessageId,
                        FileName = file.FileName,
                        FileUrl = fileUrl,
                        FileType = _fileService.GetFileExtension(file.FileName),
                        FileSize = file.Length,
                        UploadedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Repository<Messageattachment>().Add(attachment);
                }

                await _unitOfWork.Complete();
            }

            // 6. Determinar el destinatario (el otro usuario del proyecto)
            int recipientId = isClient ? project.AssignedFreelancerId!.Value : project.ClientId;

            // 7. Crear notificación para el destinatario
            await _notificationService.CreateNotificationAsync(
                userId: recipientId,
                type: "Message",
                title: "Nuevo mensaje",
                message: $"Tienes un nuevo mensaje en el proyecto: {project.Title}",
                resourceType: "Project",
                resourceId: request.ProjectId
            );

            // 8. Registrar actividad en el proyecto
            await _notificationService.LogProjectActivityAsync(
                projectId: request.ProjectId,
                userId: request.SenderId,
                activityType: "MessageSent",
                description: $"Nuevo mensaje enviado{(request.Attachments?.Any() == true ? $" con {request.Attachments.Count} adjunto(s)" : "")}"
            );

            return new SendMessageResponse
            {
                Success = true,
                Message = "Mensaje enviado exitosamente",
                MessageId = message.MessageId
            };
        }
        catch (Exception ex)
        {
            return new SendMessageResponse
            {
                Success = false,
                Message = $"Error al enviar mensaje: {ex.Message}"
            };
        }
    }
}
