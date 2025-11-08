using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Workexperience
{
    public int ExperienceId { get; set; }

    public int UserId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string? Company { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsCurrent { get; set; }

    public string? Description { get; set; }

    public virtual User User { get; set; } = null!;
}
