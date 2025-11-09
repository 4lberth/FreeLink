using System;

namespace FreeLink.Application.UseCase.Proposal
{
    public class ProposalDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int Version { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Draft";
    }
}