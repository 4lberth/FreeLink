using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<Adminactivitylog> AdminactivitylogAdmins { get; set; } = new List<Adminactivitylog>();

    public virtual ICollection<Adminactivitylog> AdminactivitylogTargetUsers { get; set; } = new List<Adminactivitylog>();

    public virtual ICollection<Contentreport> ContentreportReportedUsers { get; set; } = new List<Contentreport>();

    public virtual ICollection<Contentreport> ContentreportReporters { get; set; } = new List<Contentreport>();

    public virtual ICollection<Contentreport> ContentreportReviewedByNavigations { get; set; } = new List<Contentreport>();

    public virtual ICollection<Contract> ContractClients { get; set; } = new List<Contract>();

    public virtual ICollection<Contract> ContractFreelancers { get; set; } = new List<Contract>();

    public virtual ICollection<Contractsignature> Contractsignatures { get; set; } = new List<Contractsignature>();

    public virtual ICollection<Dispute> DisputeInitiators { get; set; } = new List<Dispute>();

    public virtual ICollection<Dispute> DisputeMediators { get; set; } = new List<Dispute>();

    public virtual ICollection<Dispute> DisputeRespondents { get; set; } = new List<Dispute>();

    public virtual ICollection<Disputemessage> Disputemessages { get; set; } = new List<Disputemessage>();

    public virtual ICollection<Escrowaccount> EscrowaccountClients { get; set; } = new List<Escrowaccount>();

    public virtual ICollection<Escrowaccount> EscrowaccountFreelancers { get; set; } = new List<Escrowaccount>();

    public virtual Freelancerprofile? Freelancerprofile { get; set; }

    public virtual ICollection<Freelancerskill> Freelancerskills { get; set; } = new List<Freelancerskill>();

    public virtual ICollection<Identityverification> IdentityverificationReviewedByNavigations { get; set; } = new List<Identityverification>();

    public virtual ICollection<Identityverification> IdentityverificationUsers { get; set; } = new List<Identityverification>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Portfolioitem> Portfolioitems { get; set; } = new List<Portfolioitem>();

    public virtual ICollection<Project> ProjectAssignedFreelancers { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectClients { get; set; } = new List<Project>();

    public virtual ICollection<Projectactivitylog> Projectactivitylogs { get; set; } = new List<Projectactivitylog>();

    public virtual ICollection<Projectapplication> Projectapplications { get; set; } = new List<Projectapplication>();

    public virtual ICollection<Projectmessage> Projectmessages { get; set; } = new List<Projectmessage>();

    public virtual ICollection<Proposalcomment> Proposalcomments { get; set; } = new List<Proposalcomment>();

    public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();

    public virtual ICollection<Review> ReviewReviewedUsers { get; set; } = new List<Review>();

    public virtual ICollection<Review> ReviewReviewers { get; set; } = new List<Review>();

    public virtual ICollection<Supportticket> SupportticketAssignedToNavigations { get; set; } = new List<Supportticket>();

    public virtual ICollection<Supportticket> SupportticketUsers { get; set; } = new List<Supportticket>();

    public virtual ICollection<Systemsetting> Systemsettings { get; set; } = new List<Systemsetting>();

    public virtual ICollection<Ticketresponse> Ticketresponses { get; set; } = new List<Ticketresponse>();

    public virtual ICollection<Transaction> TransactionFromUsers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionToUsers { get; set; } = new List<Transaction>();

    public virtual Userprofile? Userprofile { get; set; }

    public virtual ICollection<Usersanction> UsersanctionAppliedByNavigations { get; set; } = new List<Usersanction>();

    public virtual ICollection<Usersanction> UsersanctionUsers { get; set; } = new List<Usersanction>();

    public virtual Userwallet? Userwallet { get; set; }

    public virtual ICollection<Workexperience> Workexperiences { get; set; } = new List<Workexperience>();
}
