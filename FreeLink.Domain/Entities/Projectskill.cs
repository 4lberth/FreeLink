using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Projectskill
{
    public int ProjectSkillId { get; set; }

    public int ProjectId { get; set; }

    public int SkillId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
