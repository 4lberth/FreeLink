using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Usersanction
{
    public int SanctionId { get; set; }

    public int UserId { get; set; }

    public string SanctionType { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public int AppliedBy { get; set; }

    public virtual User AppliedByNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
