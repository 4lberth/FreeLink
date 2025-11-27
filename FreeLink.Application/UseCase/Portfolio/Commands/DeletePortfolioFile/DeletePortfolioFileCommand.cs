using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.DeletePortfolioFile;

public class DeletePortfolioFileCommand : IRequest<DeletePortfolioFileResponse>
{
    public int FileId { get; set; }
    public int RequestingUserId { get; set; }
}
