using FreeLink.Application.UseCase.Review.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Review.Queries.GetUserReviews;

public class GetUserReviewsQuery : IRequest<GetUserReviewsResponse>
{
    public int UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
