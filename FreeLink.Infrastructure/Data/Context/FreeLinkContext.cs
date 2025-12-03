using FreeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeLink.Infrastructure.Data.Context;

public partial class FreeLinkContext : DbContext
{
    public FreeLinkContext(DbContextOptions<FreeLinkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adminactivitylog> Adminactivitylogs { get; set; }

    public virtual DbSet<Contentreport> Contentreports { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Contractsignature> Contractsignatures { get; set; }

    public virtual DbSet<Deliverablefile> Deliverablefiles { get; set; }

    public virtual DbSet<Dispute> Disputes { get; set; }

    public virtual DbSet<Disputemessage> Disputemessages { get; set; }

    public virtual DbSet<Escrowaccount> Escrowaccounts { get; set; }

    public virtual DbSet<Freelancerprofile> Freelancerprofiles { get; set; }

    public virtual DbSet<Freelancerskill> Freelancerskills { get; set; }

    public virtual DbSet<Identityverification> Identityverifications { get; set; }

    public virtual DbSet<Messageattachment> Messageattachments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Platformcommission> Platformcommissions { get; set; }

    public virtual DbSet<Portfoliofile> Portfoliofiles { get; set; }

    public virtual DbSet<Portfolioitem> Portfolioitems { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Projectactivitylog> Projectactivitylogs { get; set; }

    public virtual DbSet<Projectapplication> Projectapplications { get; set; }

    public virtual DbSet<Projectdeliverable> Projectdeliverables { get; set; }

    public virtual DbSet<Projectmessage> Projectmessages { get; set; }

    public virtual DbSet<Projectskill> Projectskills { get; set; }

    public virtual DbSet<Proposal> Proposals { get; set; }

    public virtual DbSet<Proposalcomment> Proposalcomments { get; set; }

    public virtual DbSet<Proposalcostbreakdown> Proposalcostbreakdowns { get; set; }

    public virtual DbSet<Proposaldeliverable> Proposaldeliverables { get; set; }

    public virtual DbSet<Proposaltimeline> Proposaltimelines { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Reviewresponse> Reviewresponses { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Supportticket> Supporttickets { get; set; }

    public virtual DbSet<Systemsetting> Systemsettings { get; set; }

    public virtual DbSet<Ticketresponse> Ticketresponses { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userprofile> Userprofiles { get; set; }

    public virtual DbSet<Usersanction> Usersanctions { get; set; }

    public virtual DbSet<Userwallet> Userwallets { get; set; }

    public virtual DbSet<Workexperience> Workexperiences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adminactivitylog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("adminactivitylogs");

            entity.HasIndex(e => e.TargetUserId, "TargetUserId");

            entity.HasIndex(e => e.ActionType, "idx_action_type");

            entity.HasIndex(e => new { e.AdminId, e.CreatedAt }, "idx_admin_date");

            entity.Property(e => e.ActionType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.TargetResourceType).HasMaxLength(50);

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminactivitylogAdmins)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("adminactivitylogs_ibfk_1");

            entity.HasOne(d => d.TargetUser).WithMany(p => p.AdminactivitylogTargetUsers)
                .HasForeignKey(d => d.TargetUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("adminactivitylogs_ibfk_2");
        });

        modelBuilder.Entity<Contentreport>(entity =>
        {
            entity.HasKey(e => e.ReportId);

            entity.ToTable("contentreports");

            entity.HasIndex(e => e.ReportedMessageId, "ReportedMessageId");

            entity.HasIndex(e => e.ReportedProjectId, "ReportedProjectId");

            entity.HasIndex(e => e.ReportedUserId, "ReportedUserId");

            entity.HasIndex(e => e.ReporterId, "ReporterId");

            entity.HasIndex(e => e.ReviewedBy, "ReviewedBy");

            entity.HasIndex(e => e.ReportStatus, "idx_status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.ReportReason).HasMaxLength(255);
            entity.Property(e => e.ReportStatus)
                .HasDefaultValueSql("'Pendiente'")
                ;

            entity.HasOne(d => d.ReportedMessage).WithMany(p => p.Contentreports)
                .HasForeignKey(d => d.ReportedMessageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("contentreports_ibfk_4");

            entity.HasOne(d => d.ReportedProject).WithMany(p => p.Contentreports)
                .HasForeignKey(d => d.ReportedProjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("contentreports_ibfk_3");

            entity.HasOne(d => d.ReportedUser).WithMany(p => p.ContentreportReportedUsers)
                .HasForeignKey(d => d.ReportedUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("contentreports_ibfk_2");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ContentreportReporters)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("contentreports_ibfk_1");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.ContentreportReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("contentreports_ibfk_5");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId);

            entity.ToTable("contracts");

            entity.HasIndex(e => e.ClientId, "ClientId");

            entity.HasIndex(e => e.FreelancerId, "FreelancerId");

            entity.HasIndex(e => e.ProjectId, "ProjectId").IsUnique();

            entity.HasIndex(e => e.ProposalId, "ProposalId");

            entity.HasIndex(e => e.ContractStatus, "idx_status");

            entity.Property(e => e.ContractPdfUrl).HasMaxLength(500);
            entity.Property(e => e.ContractStatus)
                .HasDefaultValueSql("'Pendiente Firma'")
                ;
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

            entity.HasOne(d => d.Client).WithMany(p => p.ContractClients)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("contracts_ibfk_3");

            entity.HasOne(d => d.Freelancer).WithMany(p => p.ContractFreelancers)
                .HasForeignKey(d => d.FreelancerId)
                .HasConstraintName("contracts_ibfk_4");

            entity.HasOne(d => d.Project).WithOne(p => p.Contract)
                .HasForeignKey<Contract>(d => d.ProjectId)
                .HasConstraintName("contracts_ibfk_1");

            entity.HasOne(d => d.Proposal).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("contracts_ibfk_2");
        });

        modelBuilder.Entity<Contractsignature>(entity =>
        {
            entity.HasKey(e => e.SignatureId);

            entity.ToTable("contractsignatures");

            entity.HasIndex(e => e.ContractId, "ContractId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.SignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Contract).WithMany(p => p.Contractsignatures)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("contractsignatures_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Contractsignatures)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("contractsignatures_ibfk_2");
        });

        modelBuilder.Entity<Deliverablefile>(entity =>
        {
            entity.HasKey(e => e.FileId);

            entity.ToTable("deliverablefiles");

            entity.HasIndex(e => e.DeliverableId, "DeliverableId");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Deliverable).WithMany(p => p.Deliverablefiles)
                .HasForeignKey(d => d.DeliverableId)
                .HasConstraintName("deliverablefiles_ibfk_1");
        });

        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.HasKey(e => e.DisputeId);

            entity.ToTable("disputes");

            entity.HasIndex(e => e.InitiatorId, "InitiatorId");

            entity.HasIndex(e => e.MediatorId, "MediatorId");

            entity.HasIndex(e => e.ProjectId, "ProjectId");

            entity.HasIndex(e => e.RespondentId, "RespondentId");

            entity.HasIndex(e => e.DisputeStatus, "idx_status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.DisputeReason).HasMaxLength(255);
            entity.Property(e => e.DisputeStatus)
                .HasDefaultValueSql("'Abierta'")
                ;

            entity.HasOne(d => d.Initiator).WithMany(p => p.DisputeInitiators)
                .HasForeignKey(d => d.InitiatorId)
                .HasConstraintName("disputes_ibfk_2");

            entity.HasOne(d => d.Mediator).WithMany(p => p.DisputeMediators)
                .HasForeignKey(d => d.MediatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("disputes_ibfk_4");

            entity.HasOne(d => d.Project).WithMany(p => p.Disputes)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("disputes_ibfk_1");

            entity.HasOne(d => d.Respondent).WithMany(p => p.DisputeRespondents)
                .HasForeignKey(d => d.RespondentId)
                .HasConstraintName("disputes_ibfk_3");
        });

        modelBuilder.Entity<Disputemessage>(entity =>
        {
            entity.HasKey(e => e.DisputeMessageId);

            entity.ToTable("disputemessages");

            entity.HasIndex(e => e.DisputeId, "DisputeId");

            entity.HasIndex(e => e.SenderId, "SenderId");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Dispute).WithMany(p => p.Disputemessages)
                .HasForeignKey(d => d.DisputeId)
                .HasConstraintName("disputemessages_ibfk_1");

            entity.HasOne(d => d.Sender).WithMany(p => p.Disputemessages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("disputemessages_ibfk_2");
        });

        modelBuilder.Entity<Escrowaccount>(entity =>
        {
            entity.HasKey(e => e.EscrowId);

            entity.ToTable("escrowaccounts");

            entity.HasIndex(e => e.ClientId, "ClientId");

            entity.HasIndex(e => e.FreelancerId, "FreelancerId");

            entity.HasIndex(e => e.ProjectId, "ProjectId").IsUnique();

            entity.HasIndex(e => e.EscrowStatus, "idx_status");

            entity.Property(e => e.EscrowStatus)
                .HasDefaultValueSql("'Pendiente'")
                ;
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

            entity.HasOne(d => d.Client).WithMany(p => p.EscrowaccountClients)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("escrowaccounts_ibfk_2");

            entity.HasOne(d => d.Freelancer).WithMany(p => p.EscrowaccountFreelancers)
                .HasForeignKey(d => d.FreelancerId)
                .HasConstraintName("escrowaccounts_ibfk_3");

            entity.HasOne(d => d.Project).WithOne(p => p.Escrowaccount)
                .HasForeignKey<Escrowaccount>(d => d.ProjectId)
                .HasConstraintName("escrowaccounts_ibfk_1");
        });

        modelBuilder.Entity<Freelancerprofile>(entity =>
        {
            entity.HasKey(e => e.FreelancerProfileId);

            entity.ToTable("freelancerprofiles");

            entity.HasIndex(e => e.UserId, "UserId").IsUnique();

            entity.Property(e => e.AvailabilityStatus)
                .HasDefaultValueSql("'Disponible'")
                ;
            entity.Property(e => e.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.CompletedProjects)
                .HasDefaultValueSql("'0'")
                ;
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.TotalEarnings)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.TotalReviews)
                .HasDefaultValueSql("'0'")
                ;

            entity.HasOne(d => d.User).WithOne(p => p.Freelancerprofile)
                .HasForeignKey<Freelancerprofile>(d => d.UserId)
                .HasConstraintName("freelancerprofiles_ibfk_1");
        });

        modelBuilder.Entity<Freelancerskill>(entity =>
        {
            entity.HasKey(e => e.FreelancerSkillId);

            entity.ToTable("freelancerskills");

            entity.HasIndex(e => e.SkillId, "SkillId");

            entity.HasIndex(e => new { e.UserId, e.SkillId }, "unique_user_skill").IsUnique();

            entity.Property(e => e.ProficiencyLevel)
                .HasDefaultValueSql("'Intermedio'")
                ;

            entity.HasOne(d => d.Skill).WithMany(p => p.Freelancerskills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("freelancerskills_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Freelancerskills)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("freelancerskills_ibfk_1");
        });

        modelBuilder.Entity<Identityverification>(entity =>
        {
            entity.HasKey(e => e.VerificationId);

            entity.ToTable("identityverifications");

            entity.HasIndex(e => e.ReviewedBy, "ReviewedBy");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.HasIndex(e => e.VerificationStatus, "idx_status");

            entity.Property(e => e.DocumentBackUrl).HasMaxLength(500);
            entity.Property(e => e.DocumentFrontUrl).HasMaxLength(500);
            entity.Property(e => e.DocumentNumber).HasMaxLength(100);
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.SelfieUrl).HasMaxLength(500);
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.VerificationStatus)
                .HasDefaultValueSql("'Pendiente'")
                ;

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.IdentityverificationReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("identityverifications_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.IdentityverificationUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("identityverifications_ibfk_1");
        });

        modelBuilder.Entity<Messageattachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId);

            entity.ToTable("messageattachments");

            entity.HasIndex(e => e.MessageId, "MessageId");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Message).WithMany(p => p.Messageattachments)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("messageattachments_ibfk_1");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);

            entity.ToTable("notifications");

            entity.HasIndex(e => e.CreatedAt, "idx_created");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "idx_user_unread");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.IsRead).HasDefaultValueSql("'0'");
            entity.Property(e => e.NotificationType).HasMaxLength(100);
            entity.Property(e => e.RelatedResourceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_ibfk_1");
        });

        modelBuilder.Entity<Platformcommission>(entity =>
        {
            entity.HasKey(e => e.CommissionId);

            entity.ToTable("platformcommissions");

            entity.HasIndex(e => e.ProjectId, "ProjectId");

            entity.HasIndex(e => e.TransactionId, "TransactionId");

            entity.Property(e => e.Amount).HasPrecision(12, 2);
            entity.Property(e => e.CommissionRate).HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Project).WithMany(p => p.Platformcommissions)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("platformcommissions_ibfk_2");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Platformcommissions)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("platformcommissions_ibfk_1");
        });

        modelBuilder.Entity<Portfoliofile>(entity =>
        {
            entity.HasKey(e => e.FileId);

            entity.ToTable("portfoliofiles");

            entity.HasIndex(e => e.PortfolioId, "PortfolioId");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Portfolio).WithMany(p => p.Portfoliofiles)
                .HasForeignKey(d => d.PortfolioId)
                .HasConstraintName("portfoliofiles_ibfk_1");
        });

        modelBuilder.Entity<Portfolioitem>(entity =>
        {
            entity.HasKey(e => e.PortfolioId);

            entity.ToTable("portfolioitems");

            entity.HasIndex(e => new { e.UserId, e.CompletionDate }, "idx_user_date");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.ProjectUrl).HasMaxLength(255);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Portfolioitems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("portfolioitems_ibfk_1");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId);

            entity.ToTable("projects");

            entity.HasIndex(e => e.AssignedFreelancerId, "AssignedFreelancerId");

            entity.HasIndex(e => e.ClientId, "idx_client");

            entity.HasIndex(e => e.DeadlineDate, "idx_deadline");

            entity.HasIndex(e => e.ProjectStatus, "idx_status");

            entity.Property(e => e.Budget).HasPrecision(12, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.ProjectStatus)
                .HasDefaultValueSql("'Publicado'")
                ;
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.AssignedFreelancer).WithMany(p => p.ProjectAssignedFreelancers)
                .HasForeignKey(d => d.AssignedFreelancerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("projects_ibfk_2");

            entity.HasOne(d => d.Client).WithMany(p => p.ProjectClients)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("projects_ibfk_1");
        });

        modelBuilder.Entity<Projectactivitylog>(entity =>
        {
            entity.HasKey(e => e.ActivityId);

            entity.ToTable("projectactivitylog");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.HasIndex(e => new { e.ProjectId, e.CreatedAt }, "idx_project_date");

            entity.Property(e => e.ActivityType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Project).WithMany(p => p.Projectactivitylogs)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("projectactivitylog_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Projectactivitylogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("projectactivitylog_ibfk_2");
        });

        modelBuilder.Entity<Projectapplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId);

            entity.ToTable("projectapplications");

            entity.HasIndex(e => e.FreelancerId, "FreelancerId");

            entity.HasIndex(e => e.ProjectId, "idx_project");

            entity.HasIndex(e => e.ApplicationStatus, "idx_status");

            entity.HasIndex(e => new { e.ProjectId, e.FreelancerId }, "unique_application").IsUnique();

            entity.Property(e => e.ApplicationStatus)
                .HasDefaultValueSql("'Pendiente'")
                ;
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.ProposedRate).HasPrecision(10, 2);

            entity.HasOne(d => d.Freelancer).WithMany(p => p.Projectapplications)
                .HasForeignKey(d => d.FreelancerId)
                .HasConstraintName("projectapplications_ibfk_2");

            entity.HasOne(d => d.Project).WithMany(p => p.Projectapplications)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("projectapplications_ibfk_1");
        });

        modelBuilder.Entity<Projectdeliverable>(entity =>
        {
            entity.HasKey(e => e.DeliverableId);

            entity.ToTable("projectdeliverables");

            entity.HasIndex(e => new { e.ProjectId, e.DeliverableStatus }, "idx_project_status");

            entity.Property(e => e.DeliverableStatus)
                .HasDefaultValueSql("'Pendiente'")
                ;
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Project).WithMany(p => p.Projectdeliverables)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("projectdeliverables_ibfk_1");
        });

        modelBuilder.Entity<Projectmessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("projectmessages");

            entity.HasIndex(e => e.SenderId, "SenderId");

            entity.HasIndex(e => new { e.ProjectId, e.SentAt }, "idx_project_date");

            entity.Property(e => e.IsRead).HasDefaultValueSql("'0'");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Project).WithMany(p => p.Projectmessages)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("projectmessages_ibfk_1");

            entity.HasOne(d => d.Sender).WithMany(p => p.Projectmessages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("projectmessages_ibfk_2");
        });

        modelBuilder.Entity<Projectskill>(entity =>
        {
            entity.HasKey(e => e.ProjectSkillId);

            entity.ToTable("projectskills");

            entity.HasIndex(e => e.SkillId, "SkillId");

            entity.HasIndex(e => new { e.ProjectId, e.SkillId }, "unique_project_skill").IsUnique();


            entity.HasOne(d => d.Project).WithMany(p => p.Projectskills)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("projectskills_ibfk_1");

            entity.HasOne(d => d.Skill).WithMany(p => p.Projectskills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("projectskills_ibfk_2");
        });

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(e => e.ProposalId);

            entity.ToTable("proposals");

            entity.HasIndex(e => e.FreelancerId, "FreelancerId");

            entity.HasIndex(e => new { e.ProjectId, e.VersionNumber }, "idx_project_version");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.ProposalStatus)
                .HasDefaultValueSql("'Enviada'")
                ;
            entity.Property(e => e.TotalCost).HasPrecision(12, 2);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.VersionNumber)
                .HasDefaultValueSql("'1'")
                ;

            entity.HasOne(d => d.Freelancer).WithMany(p => p.Proposals)
                .HasForeignKey(d => d.FreelancerId)
                .HasConstraintName("proposals_ibfk_2");

            entity.HasOne(d => d.Project).WithMany(p => p.Proposals)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("proposals_ibfk_1");
        });

        modelBuilder.Entity<Proposalcomment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.ToTable("proposalcomments");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.HasIndex(e => new { e.ProposalId, e.CreatedAt }, "idx_proposal_date");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Proposal).WithMany(p => p.Proposalcomments)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("proposalcomments_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Proposalcomments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("proposalcomments_ibfk_2");
        });

        modelBuilder.Entity<Proposalcostbreakdown>(entity =>
        {
            entity.HasKey(e => e.CostItemId);

            entity.ToTable("proposalcostbreakdown");

            entity.HasIndex(e => e.ProposalId, "ProposalId");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.ItemDescription).HasMaxLength(255);
            entity.Property(e => e.ItemOrder)
                .HasDefaultValueSql("'0'")
                ;

            entity.HasOne(d => d.Proposal).WithMany(p => p.Proposalcostbreakdowns)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("proposalcostbreakdown_ibfk_1");
        });

        modelBuilder.Entity<Proposaldeliverable>(entity =>
        {
            entity.HasKey(e => e.DeliverableId);

            entity.ToTable("proposaldeliverables");

            entity.HasIndex(e => e.ProposalId, "ProposalId");

            entity.Property(e => e.DeliverableName).HasMaxLength(255);
            entity.Property(e => e.ItemOrder)
                .HasDefaultValueSql("'0'")
                ;

            entity.HasOne(d => d.Proposal).WithMany(p => p.Proposaldeliverables)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("proposaldeliverables_ibfk_1");
        });

        modelBuilder.Entity<Proposaltimeline>(entity =>
        {
            entity.HasKey(e => e.TimelineId);

            entity.ToTable("proposaltimeline");

            entity.HasIndex(e => e.ProposalId, "ProposalId");

            entity.Property(e => e.ItemOrder)
                .HasDefaultValueSql("'0'")
                ;
            entity.Property(e => e.MilestoneName).HasMaxLength(255);

            entity.HasOne(d => d.Proposal).WithMany(p => p.Proposaltimelines)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("proposaltimeline_ibfk_1");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);

            entity.ToTable("reviews");

            entity.HasIndex(e => e.ReviewerId, "ReviewerId");

            entity.HasIndex(e => e.Rating, "idx_rating");

            entity.HasIndex(e => e.ReviewedUserId, "idx_reviewed_user");

            entity.HasIndex(e => new { e.ProjectId, e.ReviewerId, e.ReviewedUserId }, "unique_review").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.Rating).HasPrecision(2, 1);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Project).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("reviews_ibfk_1");

            entity.HasOne(d => d.ReviewedUser).WithMany(p => p.ReviewReviewedUsers)
                .HasForeignKey(d => d.ReviewedUserId)
                .HasConstraintName("reviews_ibfk_3");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.ReviewReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("reviews_ibfk_2");
        });

        modelBuilder.Entity<Reviewresponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId);

            entity.ToTable("reviewresponses");

            entity.HasIndex(e => e.ReviewId, "ReviewId").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.Review).WithOne(p => p.Reviewresponse)
                .HasForeignKey<Reviewresponse>(d => d.ReviewId)
                .HasConstraintName("reviewresponses_ibfk_1");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId);

            entity.ToTable("skills");

            entity.HasIndex(e => e.SkillName, "SkillName").IsUnique();

            entity.HasIndex(e => e.Category, "idx_category");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.SkillName).HasMaxLength(100);
        });

        modelBuilder.Entity<Supportticket>(entity =>
        {
            entity.HasKey(e => e.TicketId);

            entity.ToTable("supporttickets");

            entity.HasIndex(e => e.AssignedTo, "AssignedTo");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.HasIndex(e => e.Priority, "idx_priority");

            entity.HasIndex(e => e.TicketStatus, "idx_status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'Media'")
                ;
            entity.Property(e => e.Subject).HasMaxLength(255);
            entity.Property(e => e.TicketStatus)
                .HasDefaultValueSql("'Abierto'")
                ;

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SupportticketAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("supporttickets_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.SupportticketUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("supporttickets_ibfk_1");
        });

        modelBuilder.Entity<Systemsetting>(entity =>
        {
            entity.HasKey(e => e.SettingId);

            entity.ToTable("systemsettings");

            entity.HasIndex(e => e.SettingKey, "SettingKey").IsUnique();

            entity.HasIndex(e => e.UpdatedBy, "UpdatedBy");

            entity.Property(e => e.SettingKey).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.Systemsettings)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("systemsettings_ibfk_1");
        });

        modelBuilder.Entity<Ticketresponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId);

            entity.ToTable("ticketresponses");

            entity.HasIndex(e => e.ResponderId, "ResponderId");

            entity.HasIndex(e => e.TicketId, "TicketId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.IsStaffResponse).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Responder).WithMany(p => p.Ticketresponses)
                .HasForeignKey(d => d.ResponderId)
                .HasConstraintName("ticketresponses_ibfk_2");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Ticketresponses)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("ticketresponses_ibfk_1");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);

            entity.ToTable("transactions");

            entity.HasIndex(e => e.EscrowId, "EscrowId");

            entity.HasIndex(e => e.CreatedAt, "idx_created");

            entity.HasIndex(e => e.FromUserId, "idx_from_user");

            entity.HasIndex(e => e.ToUserId, "idx_to_user");

            entity.HasIndex(e => new { e.TransactionType, e.TransactionStatus }, "idx_type_status");

            entity.Property(e => e.Amount).HasPrecision(12, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.ReceiptUrl).HasMaxLength(500);
            entity.Property(e => e.TransactionStatus)
                .HasDefaultValueSql("'Pendiente'")
                ;

            entity.HasOne(d => d.Escrow).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.EscrowId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("transactions_ibfk_1");

            entity.HasOne(d => d.FromUser).WithMany(p => p.TransactionFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("transactions_ibfk_2");

            entity.HasOne(d => d.ToUser).WithMany(p => p.TransactionToUsers)
                .HasForeignKey(d => d.ToUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("transactions_ibfk_3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.UserType, "idx_user_type");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsVerified).HasDefaultValueSql("'0'");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;
        });

        modelBuilder.Entity<Userprofile>(entity =>
        {
            entity.HasKey(e => e.ProfileId);

            entity.ToTable("userprofiles");

            entity.HasIndex(e => e.UserId, "UserId").IsUnique();

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ProfilePicture).HasMaxLength(255);

            entity.HasOne(d => d.User).WithOne(p => p.Userprofile)
                .HasForeignKey<Userprofile>(d => d.UserId)
                .HasConstraintName("userprofiles_ibfk_1");
        });

        modelBuilder.Entity<Usersanction>(entity =>
        {
            entity.HasKey(e => e.SanctionId);

            entity.ToTable("usersanctions");

            entity.HasIndex(e => e.AppliedBy, "AppliedBy");

            entity.HasIndex(e => new { e.UserId, e.IsActive }, "idx_user_active");

            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.AppliedByNavigation).WithMany(p => p.UsersanctionAppliedByNavigations)
                .HasForeignKey(d => d.AppliedBy)
                .HasConstraintName("usersanctions_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.UsersanctionUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("usersanctions_ibfk_1");
        });

        modelBuilder.Entity<Userwallet>(entity =>
        {
            entity.HasKey(e => e.WalletId);

            entity.ToTable("userwallets");

            entity.HasIndex(e => e.UserId, "UserId").IsUnique();

            entity.Property(e => e.Balance)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.PendingBalance)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.TotalEarnings)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.TotalSpent)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                ;

            entity.HasOne(d => d.User).WithOne(p => p.Userwallet)
                .HasForeignKey<Userwallet>(d => d.UserId)
                .HasConstraintName("userwallets_ibfk_1");
        });

        modelBuilder.Entity<Workexperience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId);

            entity.ToTable("workexperience");

            entity.HasIndex(e => new { e.UserId, e.IsCurrent }, "idx_user_current");

            entity.Property(e => e.Company).HasMaxLength(255);
            entity.Property(e => e.IsCurrent).HasDefaultValueSql("'0'");
            entity.Property(e => e.JobTitle).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Workexperiences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("workexperience_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
