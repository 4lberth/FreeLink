using FreeLink.Application.UseCase.Messages.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Messages.Queries.GetProjectMessages;

public class GetProjectMessagesHandler : IRequestHandler<GetProjectMessagesQuery, GetProjectMessagesResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectMessagesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProjectMessagesResponse> Handle(GetProjectMessagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar que el proyecto existe
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new GetProjectMessagesResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 2. Verificar que el usuario es parte del proyecto
            if (project.ClientId != request.RequestingUserId && project.AssignedFreelancerId != request.RequestingUserId)
            {
                return new GetProjectMessagesResponse
                {
                    Success = false,
                    Message = "No tienes permiso para ver los mensajes de este proyecto"
                };
            }

            // 3. Obtener todos los mensajes del proyecto
            var messagesQuery = await _unitOfWork.Repository<Projectmessage>()
                .GetAsync(m => m.ProjectId == request.ProjectId);

            var messages = messagesQuery.OrderBy(m => m.SentAt).ToList();

            // 4. Obtener usuarios para nombres
            var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var usersQuery = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetAsync(u => senderIds.Contains(u.UserId));
            var users = usersQuery.ToList();

            // 5. Obtener attachments de todos los mensajes
            var messageIds = messages.Select(m => m.MessageId).ToList();
            var attachmentsQuery = await _unitOfWork.Repository<Messageattachment>()
                .GetAsync(a => messageIds.Contains(a.MessageId));
            var attachments = attachmentsQuery.ToList();

            // 6. Mapear a DTOs
            var messageDtos = messages.Select(m =>
            {
                var sender = users.FirstOrDefault(u => u.UserId == m.SenderId);
                var messageAttachments = attachments.Where(a => a.MessageId == m.MessageId).ToList();

                return new MessageDto
                {
                    MessageId = m.MessageId,
                    ProjectId = m.ProjectId,
                    SenderId = m.SenderId,
                    SenderName = sender != null ? $"{sender.Email}" : "Usuario desconocido",
                    SenderRole = m.SenderId == project.ClientId ? "Cliente" : "Freelancer",
                    Content = m.MessageText,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead ?? false,
                    Attachments = messageAttachments.Select(a => new AttachmentDto
                    {
                        AttachmentId = a.AttachmentId,
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        FileSize = a.FileSize ?? 0,
                        UploadedAt = a.UploadedAt
                    }).ToList()
                };
            }).ToList();

            return new GetProjectMessagesResponse
            {
                Success = true,
                Message = "Mensajes obtenidos exitosamente",
                Messages = messageDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProjectMessagesResponse
            {
                Success = false,
                Message = $"Error al obtener mensajes: {ex.Message}"
            };
        }
    }
}
