namespace FreeLink.Application.UseCase.Project.DTOs
{
    public sealed class DeliverableStatusSummaryDto
    {
        public int Pending { get; set; }
        public int Sent { get; set; }
        public int InReview { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
}