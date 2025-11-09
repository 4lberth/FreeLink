using System;

namespace FreeLink.Application.UseCase.Proposal
{
    public class ProposalCreateDto
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
    }
}