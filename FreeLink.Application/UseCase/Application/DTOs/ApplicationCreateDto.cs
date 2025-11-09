namespace FreeLink.Application.UseCase.Application.DTOs
{
    public class ApplicationCreateDto
    {
        // ProjectId vendrá de la URL, FreelancerId idealmente del token (lo simulamos por ahora)
        public int FreelancerId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public decimal? ProposedRate { get; set; }
        public int? EstimatedDuration { get; set; }
    }
}