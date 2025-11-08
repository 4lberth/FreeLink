using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Proposalcomment
{
    public int CommentId { get; set; }

    public int ProposalId { get; set; }

    public int UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Proposal Proposal { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
