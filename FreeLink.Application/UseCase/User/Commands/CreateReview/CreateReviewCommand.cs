using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.CreateReview;

public class CreateReviewCommand : IRequest<CreateReviewResponse>
{
    public int ProjectId { get; set; }
    public int ReviewerId { get; set; }
    public int ReviewedUserId { get; set; }
    public decimal Rating { get; set; } // 1.0 a 5.0
    public string? ReviewText { get; set; }
    public string ReviewType { get; set; } = string.Empty; // "Cliente a Freelancer" o "Freelancer a Cliente"
}

