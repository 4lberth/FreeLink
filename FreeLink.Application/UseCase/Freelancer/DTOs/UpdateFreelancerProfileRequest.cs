namespace FreeLink.Application.UseCase.Freelancer.DTOs;

public class UpdateFreelancerProfileRequest
{
    public decimal? HourlyRate { get; set; }
    public string? AvailabilityStatus { get; set; }
    public string? ProfessionalTitle { get; set; }
}