using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.AddProposalComment;

public class AddProposalCommentHandler : IRequestHandler<AddProposalCommentCommand, AddProposalCommentResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public AddProposalCommentHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<AddProposalCommentResponse> Handle(AddProposalCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la propuesta
            var proposal = await _unitOfWork.Repository<Proposal>().GetById(request.ProposalId);
            if (proposal == null)
            {
                return new AddProposalCommentResponse
                {
                    Success = false,
                    Message = "Propuesta no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(proposal.ProjectId);
            if (project == null)
            {
                return new AddProposalCommentResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Validar que el usuario puede comentar (cliente dueño o freelancer dueño)
            bool isClient = project.ClientId == request.UserId;
            bool isFreelancer = proposal.FreelancerId == request.UserId;

            if (!isClient && !isFreelancer)
            {
                return new AddProposalCommentResponse
                {
                    Success = false,
                    Message = "Solo el cliente o el freelancer de esta propuesta pueden agregar comentarios"
                };
            }

            // 4. Validar que el comentario no esté vacío
            if (string.IsNullOrWhiteSpace(request.CommentText))
            {
                return new AddProposalCommentResponse
                {
                    Success = false,
                    Message = "El comentario no puede estar vacío"
                };
            }

            // 5. Crear el comentario
            var comment = new Proposalcomment
            {
                ProposalId = request.ProposalId,
                UserId = request.UserId,
                CommentText = request.CommentText,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Proposalcomment>().Add(comment);
            await _unitOfWork.Complete();

            // 6. Determinar a quién notificar (la otra parte)
            int recipientId = isClient ? proposal.FreelancerId : project.ClientId;
            string recipientType = isClient ? "freelancer" : "cliente";

            // 7. Notificar a la otra parte
            await _notificationService.CreateNotificationAsync(
                userId: recipientId,
                type: "Proposal",
                title: "Nuevo comentario en propuesta",
                message: $"El {recipientType} ha dejado un comentario en la propuesta del proyecto '{project.Title}'. Revisa los comentarios para continuar la negociación.",
                resourceType: "Proposal",
                resourceId: proposal.ProposalId
            );

            return new AddProposalCommentResponse
            {
                Success = true,
                Message = "Comentario agregado exitosamente",
                CommentId = comment.CommentId
            };
        }
        catch (Exception ex)
        {
            return new AddProposalCommentResponse
            {
                Success = false,
                Message = $"Error al agregar comentario: {ex.Message}"
            };
        }
    }
}
