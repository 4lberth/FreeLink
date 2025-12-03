using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetDisputeDetails;

public class GetDisputeDetailsHandler : IRequestHandler<GetDisputeDetailsQuery, GetDisputeDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDisputeDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetDisputeDetailsResponse> Handle(GetDisputeDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetDisputeDetailsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver detalles de disputas"
                };
            }

            // Obtener la disputa
            var dispute = await _unitOfWork.Repository<Dispute>().GetById(request.DisputeId);
            if (dispute == null)
            {
                return new GetDisputeDetailsResponse
                {
                    Success = false,
                    Message = "Disputa no encontrada"
                };
            }

            // Obtener usuarios relacionados
            var initiator = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(dispute.InitiatorId);
            var respondent = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(dispute.RespondentId);

            FreeLink.Domain.Entities.User? mediator = null;
            if (dispute.MediatorId.HasValue)
            {
                mediator = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(dispute.MediatorId.Value);
            }

            // Obtener proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(dispute.ProjectId);

            // Obtener mensajes de la disputa
            var allMessages = await _unitOfWork.Repository<Disputemessage>().GetAll();
            var disputeMessages = allMessages
                .Where(m => m.DisputeId == request.DisputeId)
                .OrderBy(m => m.SentAt)
                .ToList();

            // Obtener usuarios que enviaron mensajes
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear mensajes a DTOs
            var messageDtos = disputeMessages.Select(m =>
            {
                var sender = usersList.FirstOrDefault(u => u.UserId == m.SenderId);
                return new DisputeMessageDto
                {
                    DisputeMessageId = m.DisputeMessageId,
                    SenderId = m.SenderId,
                    SenderName = sender?.Email ?? "Usuario no encontrado",
                    MessageText = m.MessageText,
                    SentAt = m.SentAt
                };
            }).ToList();

            var disputeDetails = new DisputeDetailsDto
            {
                DisputeId = dispute.DisputeId,
                ProjectId = dispute.ProjectId,
                ProjectTitle = project?.Title,
                InitiatorId = dispute.InitiatorId,
                InitiatorName = initiator?.Email ?? "Usuario no encontrado",
                InitiatorEmail = initiator?.Email ?? "",
                RespondentId = dispute.RespondentId,
                RespondentName = respondent?.Email ?? "Usuario no encontrado",
                RespondentEmail = respondent?.Email ?? "",
                DisputeReason = dispute.DisputeReason,
                DisputeDescription = dispute.DisputeDescription,
                DisputeStatus = dispute.DisputeStatus,
                CreatedAt = dispute.CreatedAt,
                MediatorId = dispute.MediatorId,
                MediatorName = mediator?.Email,
                ResolvedAt = dispute.ResolvedAt,
                Resolution = dispute.Resolution,
                Messages = messageDtos
            };

            return new GetDisputeDetailsResponse
            {
                Success = true,
                Message = "Detalles de disputa obtenidos exitosamente",
                Data = disputeDetails
            };
        }
        catch (Exception ex)
        {
            return new GetDisputeDetailsResponse
            {
                Success = false,
                Message = $"Error al obtener detalles de disputa: {ex.Message}"
            };
        }
    }
}
