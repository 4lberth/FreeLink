using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Skill
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = null!;

    public string? Category { get; set; }

    public virtual ICollection<Freelancerskill> Freelancerskills { get; set; } = new List<Freelancerskill>();

    public virtual ICollection<Projectskill> Projectskills { get; set; } = new List<Projectskill>();
}
