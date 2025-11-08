namespace FreeLink.Application.UseCase.User.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? FreelancerProfileId { get; set; }
}

