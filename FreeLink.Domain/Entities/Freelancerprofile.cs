using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Freelancerprofile
{
    public int FreelancerProfileId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public decimal? HourlyRate { get; set; }

    public int? YearsOfExperience { get; set; }

    public string? AvailabilityStatus { get; set; }

    public decimal? TotalEarnings { get; set; }

    public int? CompletedProjects { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalReviews { get; set; }

    public virtual User User { get; set; } = null!;
}
