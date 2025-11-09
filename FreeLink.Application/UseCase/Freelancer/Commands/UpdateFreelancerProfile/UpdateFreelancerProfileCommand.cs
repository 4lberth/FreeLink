using MediatR;

namespace FreeLink.Application.UseCase.Freelancer.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommand : IRequest<UpdateFreelancerProfileResponse>
{
    public int FreelancerId { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? AvailabilityStatus { get; set; }
    public string? ProfessionalTitle { get; set; }
    
    // Usuario que hace la petición (desde el token)
    public int RequestingUserId { get; set; }
    public string RequestingUserRole { get; set; } = string.Empty;
}