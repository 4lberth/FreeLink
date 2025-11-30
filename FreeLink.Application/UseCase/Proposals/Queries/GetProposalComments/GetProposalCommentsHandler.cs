using FreeLink.Application.UseCase.Proposals.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalComments;

public class GetProposalCommentsHandler : IRequestHandler<GetProposalCommentsQuery, GetProposalCommentsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProposalCommentsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProposalCommentsResponse> Handle(GetProposalCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar la propuesta
            var proposal = await _unitOfWork.Repository<Proposal>().GetById(request.ProposalId);
            if (proposal == null)
            {
                return new GetProposalCommentsResponse
                {
                    Success = false,
                    Message = "Propuesta no encontrada"
                };
            }

            // 2. Buscar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(proposal.ProjectId);
            if (project == null)
            {
                return new GetProposalCommentsResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 3. Verificar permisos (solo cliente dueño o freelancer dueño)
            if (request.RequestingUserId.HasValue)
            {
                bool isClient = project.ClientId == request.RequestingUserId.Value;
                bool isFreelancer = proposal.FreelancerId == request.RequestingUserId.Value;

                if (!isClient && !isFreelancer)
                {
                    return new GetProposalCommentsResponse
                    {
                        Success = false,
                        Message = "No tienes permiso para ver los comentarios de esta propuesta"
                    };
                }
            }

            // 4. Obtener todos los comentarios
            var comments = await _unitOfWork.Repository<Proposalcomment>()
                .GetAsync(c => c.ProposalId == request.ProposalId);

            var commentsList = comments.OrderBy(c => c.CreatedAt).ToList();

            // 5. Construir DTOs con información de usuarios
            var commentDtos = new List<ProposalCommentDto>();

            foreach (var comment in commentsList)
            {
                var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(comment.UserId);
                var userProfile = await _unitOfWork.Repository<Userprofile>()
                    .GetFirstOrDefaultAsync(p => p.UserId == comment.UserId);

                string userName = userProfile != null
                    ? $"{userProfile.FirstName} {userProfile.LastName}"
                    : user?.Email ?? "Usuario desconocido";

                string userType = user?.UserType ?? "Unknown";

                commentDtos.Add(new ProposalCommentDto
                {
                    CommentId = comment.CommentId,
                    ProposalId = comment.ProposalId,
                    UserId = comment.UserId,
                    UserName = userName,
                    UserType = userType,
                    CommentText = comment.CommentText,
                    CreatedAt = comment.CreatedAt
                });
            }

            return new GetProposalCommentsResponse
            {
                Success = true,
                Message = $"{commentDtos.Count} comentario(s) encontrado(s)",
                Comments = commentDtos
            };
        }
        catch (Exception ex)
        {
            return new GetProposalCommentsResponse
            {
                Success = false,
                Message = $"Error al obtener comentarios: {ex.Message}"
            };
        }
    }
}
