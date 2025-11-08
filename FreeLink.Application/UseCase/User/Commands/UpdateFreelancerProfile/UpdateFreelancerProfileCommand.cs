using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommand : IRequest<UpdateFreelancerProfileResponse>
{
    public int UserId { get; set; }
    public string? Title { get; set; }
    public decimal? HourlyRate { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? AvailabilityStatus { get; set; }
}

