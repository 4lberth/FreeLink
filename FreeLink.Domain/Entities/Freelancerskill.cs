using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Freelancerskill
{
    public int FreelancerSkillId { get; set; }

    public int UserId { get; set; }

    public int SkillId { get; set; }

    public string? ProficiencyLevel { get; set; }

    public virtual Skill Skill { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
