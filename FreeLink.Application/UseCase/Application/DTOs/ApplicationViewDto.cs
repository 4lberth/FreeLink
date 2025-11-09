using System;

namespace FreeLink.Application.UseCase.Application.DTOs
{
    public class ApplicationViewDto
    {
        public int ApplicationId { get; set; }
        public int ProjectId { get; set; }
        public int FreelancerId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public decimal? ProposedRate { get; set; }
        public int? EstimatedDuration { get; set; }
        public string ApplicationStatus { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
    }
}