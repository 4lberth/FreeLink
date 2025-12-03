using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FreeLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SkillName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'1'"),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'0'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "adminactivitylogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminId = table.Column<int>(type: "integer", nullable: false),
                    ActionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActionDescription = table.Column<string>(type: "text", nullable: true),
                    TargetUserId = table.Column<int>(type: "integer", nullable: true),
                    TargetResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TargetResourceId = table.Column<int>(type: "integer", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminactivitylogs", x => x.LogId);
                    table.ForeignKey(
                        name: "adminactivitylogs_ibfk_1",
                        column: x => x.AdminId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "adminactivitylogs_ibfk_2",
                        column: x => x.TargetUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "freelancerprofiles",
                columns: table => new
                {
                    FreelancerProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: true),
                    AvailabilityStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Disponible'"),
                    TotalEarnings = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    CompletedProjects = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    TotalReviews = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_freelancerprofiles", x => x.FreelancerProfileId);
                    table.ForeignKey(
                        name: "freelancerprofiles_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "freelancerskills",
                columns: table => new
                {
                    FreelancerSkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    ProficiencyLevel = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Intermedio'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_freelancerskills", x => x.FreelancerSkillId);
                    table.ForeignKey(
                        name: "freelancerskills_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "freelancerskills_ibfk_2",
                        column: x => x.SkillId,
                        principalTable: "skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityverifications",
                columns: table => new
                {
                    VerificationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DocumentFrontUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DocumentBackUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SelfieUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente'"),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<int>(type: "integer", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityverifications", x => x.VerificationId);
                    table.ForeignKey(
                        name: "identityverifications_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "identityverifications_ibfk_2",
                        column: x => x.ReviewedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'0'"),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RelatedResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelatedResourceId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "notifications_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "portfolioitems",
                columns: table => new
                {
                    PortfolioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CompletionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portfolioitems", x => x.PortfolioId);
                    table.ForeignKey(
                        name: "portfolioitems_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Budget = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    DeadlineDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProjectStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Publicado'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    AssignedFreelancerId = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "projects_ibfk_1",
                        column: x => x.ClientId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "projects_ibfk_2",
                        column: x => x.AssignedFreelancerId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "supporttickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TicketStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Abierto'"),
                    Priority = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Media'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    AssignedTo = table.Column<int>(type: "integer", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supporttickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "supporttickets_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "supporttickets_ibfk_2",
                        column: x => x.AssignedTo,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "systemsettings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SettingKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemsettings", x => x.SettingId);
                    table.ForeignKey(
                        name: "systemsettings_ibfk_1",
                        column: x => x.UpdatedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "userprofiles",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProfilePicture = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userprofiles", x => x.ProfileId);
                    table.ForeignKey(
                        name: "userprofiles_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usersanctions",
                columns: table => new
                {
                    SanctionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SanctionType = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'1'"),
                    AppliedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usersanctions", x => x.SanctionId);
                    table.ForeignKey(
                        name: "usersanctions_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "usersanctions_ibfk_2",
                        column: x => x.AppliedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userwallets",
                columns: table => new
                {
                    WalletId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    PendingBalance = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    TotalEarnings = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    TotalSpent = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userwallets", x => x.WalletId);
                    table.ForeignKey(
                        name: "userwallets_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workexperience",
                columns: table => new
                {
                    ExperienceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Company = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'0'"),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workexperience", x => x.ExperienceId);
                    table.ForeignKey(
                        name: "workexperience_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "portfoliofiles",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PortfolioId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portfoliofiles", x => x.FileId);
                    table.ForeignKey(
                        name: "portfoliofiles_ibfk_1",
                        column: x => x.PortfolioId,
                        principalTable: "portfolioitems",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "disputes",
                columns: table => new
                {
                    DisputeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    InitiatorId = table.Column<int>(type: "integer", nullable: false),
                    RespondentId = table.Column<int>(type: "integer", nullable: false),
                    DisputeReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisputeDescription = table.Column<string>(type: "text", nullable: false),
                    DisputeStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Abierta'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    MediatorId = table.Column<int>(type: "integer", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Resolution = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disputes", x => x.DisputeId);
                    table.ForeignKey(
                        name: "disputes_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "disputes_ibfk_2",
                        column: x => x.InitiatorId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "disputes_ibfk_3",
                        column: x => x.RespondentId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "disputes_ibfk_4",
                        column: x => x.MediatorId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "escrowaccounts",
                columns: table => new
                {
                    EscrowId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    FreelancerId = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    EscrowStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente'"),
                    DepositedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escrowaccounts", x => x.EscrowId);
                    table.ForeignKey(
                        name: "escrowaccounts_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "escrowaccounts_ibfk_2",
                        column: x => x.ClientId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "escrowaccounts_ibfk_3",
                        column: x => x.FreelancerId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projectactivitylog",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActivityDescription = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectactivitylog", x => x.ActivityId);
                    table.ForeignKey(
                        name: "projectactivitylog_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "projectactivitylog_ibfk_2",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "projectapplications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    FreelancerId = table.Column<int>(type: "integer", nullable: false),
                    CoverLetter = table.Column<string>(type: "text", nullable: true),
                    ProposedRate = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    EstimatedDuration = table.Column<int>(type: "integer", nullable: true),
                    ApplicationStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente'"),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectapplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "projectapplications_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "projectapplications_ibfk_2",
                        column: x => x.FreelancerId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projectdeliverables",
                columns: table => new
                {
                    DeliverableId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DeliverableStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente'"),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewComments = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectdeliverables", x => x.DeliverableId);
                    table.ForeignKey(
                        name: "projectdeliverables_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projectmessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    MessageText = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsRead = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'0'"),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectmessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "projectmessages_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "projectmessages_ibfk_2",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projectskills",
                columns: table => new
                {
                    ProjectSkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectskills", x => x.ProjectSkillId);
                    table.ForeignKey(
                        name: "projectskills_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "projectskills_ibfk_2",
                        column: x => x.SkillId,
                        principalTable: "skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proposals",
                columns: table => new
                {
                    ProposalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    FreelancerId = table.Column<int>(type: "integer", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'1'"),
                    TotalCost = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ProposalStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Enviada'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposals", x => x.ProposalId);
                    table.ForeignKey(
                        name: "proposals_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "proposals_ibfk_2",
                        column: x => x.FreelancerId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ReviewerId = table.Column<int>(type: "integer", nullable: false),
                    ReviewedUserId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: false),
                    ReviewText = table.Column<string>(type: "text", nullable: true),
                    ReviewType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "reviews_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "reviews_ibfk_2",
                        column: x => x.ReviewerId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "reviews_ibfk_3",
                        column: x => x.ReviewedUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticketresponses",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    ResponderId = table.Column<int>(type: "integer", nullable: false),
                    ResponseText = table.Column<string>(type: "text", nullable: false),
                    IsStaffResponse = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "'0'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticketresponses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "ticketresponses_ibfk_1",
                        column: x => x.TicketId,
                        principalTable: "supporttickets",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ticketresponses_ibfk_2",
                        column: x => x.ResponderId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "disputemessages",
                columns: table => new
                {
                    DisputeMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisputeId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    MessageText = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disputemessages", x => x.DisputeMessageId);
                    table.ForeignKey(
                        name: "disputemessages_ibfk_1",
                        column: x => x.DisputeId,
                        principalTable: "disputes",
                        principalColumn: "DisputeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "disputemessages_ibfk_2",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EscrowId = table.Column<int>(type: "integer", nullable: true),
                    FromUserId = table.Column<int>(type: "integer", nullable: true),
                    ToUserId = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    TransactionStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente'"),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "transactions_ibfk_1",
                        column: x => x.EscrowId,
                        principalTable: "escrowaccounts",
                        principalColumn: "EscrowId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "transactions_ibfk_2",
                        column: x => x.FromUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "transactions_ibfk_3",
                        column: x => x.ToUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "deliverablefiles",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeliverableId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliverablefiles", x => x.FileId);
                    table.ForeignKey(
                        name: "deliverablefiles_ibfk_1",
                        column: x => x.DeliverableId,
                        principalTable: "projectdeliverables",
                        principalColumn: "DeliverableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contentreports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReporterId = table.Column<int>(type: "integer", nullable: false),
                    ReportedUserId = table.Column<int>(type: "integer", nullable: true),
                    ReportedProjectId = table.Column<int>(type: "integer", nullable: true),
                    ReportedMessageId = table.Column<int>(type: "integer", nullable: true),
                    ReportReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ReportDescription = table.Column<string>(type: "text", nullable: true),
                    ReportStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<int>(type: "integer", nullable: true),
                    Resolution = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contentreports", x => x.ReportId);
                    table.ForeignKey(
                        name: "contentreports_ibfk_1",
                        column: x => x.ReporterId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contentreports_ibfk_2",
                        column: x => x.ReportedUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "contentreports_ibfk_3",
                        column: x => x.ReportedProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "contentreports_ibfk_4",
                        column: x => x.ReportedMessageId,
                        principalTable: "projectmessages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "contentreports_ibfk_5",
                        column: x => x.ReviewedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "messageattachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageattachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "messageattachments_ibfk_1",
                        column: x => x.MessageId,
                        principalTable: "projectmessages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    FreelancerId = table.Column<int>(type: "integer", nullable: false),
                    ContractStatus = table.Column<string>(type: "text", nullable: true, defaultValueSql: "'Pendiente Firma'"),
                    TotalAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ClientSignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FreelancerSignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractPdfUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.ContractId);
                    table.ForeignKey(
                        name: "contracts_ibfk_1",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contracts_ibfk_2",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "ProposalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contracts_ibfk_3",
                        column: x => x.ClientId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contracts_ibfk_4",
                        column: x => x.FreelancerId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proposalcomments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CommentText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposalcomments", x => x.CommentId);
                    table.ForeignKey(
                        name: "proposalcomments_ibfk_1",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "ProposalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "proposalcomments_ibfk_2",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proposalcostbreakdown",
                columns: table => new
                {
                    CostItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    ItemDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ItemOrder = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposalcostbreakdown", x => x.CostItemId);
                    table.ForeignKey(
                        name: "proposalcostbreakdown_ibfk_1",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "ProposalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proposaldeliverables",
                columns: table => new
                {
                    DeliverableId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    DeliverableName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ItemOrder = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposaldeliverables", x => x.DeliverableId);
                    table.ForeignKey(
                        name: "proposaldeliverables_ibfk_1",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "ProposalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proposaltimeline",
                columns: table => new
                {
                    TimelineId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    MilestoneName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EstimatedDuration = table.Column<int>(type: "integer", nullable: true),
                    ItemOrder = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposaltimeline", x => x.TimelineId);
                    table.ForeignKey(
                        name: "proposaltimeline_ibfk_1",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "ProposalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviewresponses",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    ResponseText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviewresponses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "reviewresponses_ibfk_1",
                        column: x => x.ReviewId,
                        principalTable: "reviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "platformcommissions",
                columns: table => new
                {
                    CommissionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platformcommissions", x => x.CommissionId);
                    table.ForeignKey(
                        name: "platformcommissions_ibfk_1",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "platformcommissions_ibfk_2",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contractsignatures",
                columns: table => new
                {
                    SignatureId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SignatureData = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    SignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contractsignatures", x => x.SignatureId);
                    table.ForeignKey(
                        name: "contractsignatures_ibfk_1",
                        column: x => x.ContractId,
                        principalTable: "contracts",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contractsignatures_ibfk_2",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_action_type",
                table: "adminactivitylogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "idx_admin_date",
                table: "adminactivitylogs",
                columns: new[] { "AdminId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "TargetUserId",
                table: "adminactivitylogs",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "idx_status",
                table: "contentreports",
                column: "ReportStatus");

            migrationBuilder.CreateIndex(
                name: "ReportedMessageId",
                table: "contentreports",
                column: "ReportedMessageId");

            migrationBuilder.CreateIndex(
                name: "ReportedProjectId",
                table: "contentreports",
                column: "ReportedProjectId");

            migrationBuilder.CreateIndex(
                name: "ReportedUserId",
                table: "contentreports",
                column: "ReportedUserId");

            migrationBuilder.CreateIndex(
                name: "ReporterId",
                table: "contentreports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "ReviewedBy",
                table: "contentreports",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "ClientId",
                table: "contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "FreelancerId",
                table: "contracts",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "idx_status1",
                table: "contracts",
                column: "ContractStatus");

            migrationBuilder.CreateIndex(
                name: "ProjectId",
                table: "contracts",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ProposalId",
                table: "contracts",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "ContractId",
                table: "contractsignatures",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "UserId",
                table: "contractsignatures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "DeliverableId",
                table: "deliverablefiles",
                column: "DeliverableId");

            migrationBuilder.CreateIndex(
                name: "DisputeId",
                table: "disputemessages",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "SenderId",
                table: "disputemessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "idx_status2",
                table: "disputes",
                column: "DisputeStatus");

            migrationBuilder.CreateIndex(
                name: "InitiatorId",
                table: "disputes",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "MediatorId",
                table: "disputes",
                column: "MediatorId");

            migrationBuilder.CreateIndex(
                name: "ProjectId1",
                table: "disputes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "RespondentId",
                table: "disputes",
                column: "RespondentId");

            migrationBuilder.CreateIndex(
                name: "ClientId1",
                table: "escrowaccounts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "FreelancerId1",
                table: "escrowaccounts",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "idx_status3",
                table: "escrowaccounts",
                column: "EscrowStatus");

            migrationBuilder.CreateIndex(
                name: "ProjectId2",
                table: "escrowaccounts",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserId1",
                table: "freelancerprofiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "SkillId",
                table: "freelancerskills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "unique_user_skill",
                table: "freelancerskills",
                columns: new[] { "UserId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_status4",
                table: "identityverifications",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "ReviewedBy1",
                table: "identityverifications",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "UserId2",
                table: "identityverifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "MessageId",
                table: "messageattachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "idx_created",
                table: "notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_user_unread",
                table: "notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "ProjectId3",
                table: "platformcommissions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "TransactionId",
                table: "platformcommissions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "PortfolioId",
                table: "portfoliofiles",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "idx_user_date",
                table: "portfolioitems",
                columns: new[] { "UserId", "CompletionDate" });

            migrationBuilder.CreateIndex(
                name: "idx_project_date",
                table: "projectactivitylog",
                columns: new[] { "ProjectId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "UserId3",
                table: "projectactivitylog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "FreelancerId2",
                table: "projectapplications",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "idx_project",
                table: "projectapplications",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "idx_status6",
                table: "projectapplications",
                column: "ApplicationStatus");

            migrationBuilder.CreateIndex(
                name: "unique_application",
                table: "projectapplications",
                columns: new[] { "ProjectId", "FreelancerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_project_status",
                table: "projectdeliverables",
                columns: new[] { "ProjectId", "DeliverableStatus" });

            migrationBuilder.CreateIndex(
                name: "idx_project_date1",
                table: "projectmessages",
                columns: new[] { "ProjectId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "SenderId1",
                table: "projectmessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "AssignedFreelancerId",
                table: "projects",
                column: "AssignedFreelancerId");

            migrationBuilder.CreateIndex(
                name: "idx_client",
                table: "projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "idx_deadline",
                table: "projects",
                column: "DeadlineDate");

            migrationBuilder.CreateIndex(
                name: "idx_status5",
                table: "projects",
                column: "ProjectStatus");

            migrationBuilder.CreateIndex(
                name: "SkillId1",
                table: "projectskills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "unique_project_skill",
                table: "projectskills",
                columns: new[] { "ProjectId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_proposal_date",
                table: "proposalcomments",
                columns: new[] { "ProposalId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "UserId4",
                table: "proposalcomments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ProposalId1",
                table: "proposalcostbreakdown",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "ProposalId2",
                table: "proposaldeliverables",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "FreelancerId3",
                table: "proposals",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "idx_project_version",
                table: "proposals",
                columns: new[] { "ProjectId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "ProposalId3",
                table: "proposaltimeline",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "ReviewId",
                table: "reviewresponses",
                column: "ReviewId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_rating",
                table: "reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "idx_reviewed_user",
                table: "reviews",
                column: "ReviewedUserId");

            migrationBuilder.CreateIndex(
                name: "ReviewerId",
                table: "reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "unique_review",
                table: "reviews",
                columns: new[] { "ProjectId", "ReviewerId", "ReviewedUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_category",
                table: "skills",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "SkillName",
                table: "skills",
                column: "SkillName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AssignedTo",
                table: "supporttickets",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "idx_priority",
                table: "supporttickets",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "idx_status7",
                table: "supporttickets",
                column: "TicketStatus");

            migrationBuilder.CreateIndex(
                name: "UserId5",
                table: "supporttickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "SettingKey",
                table: "systemsettings",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UpdatedBy",
                table: "systemsettings",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "ResponderId",
                table: "ticketresponses",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "TicketId",
                table: "ticketresponses",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "EscrowId",
                table: "transactions",
                column: "EscrowId");

            migrationBuilder.CreateIndex(
                name: "idx_created1",
                table: "transactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_from_user",
                table: "transactions",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "idx_to_user",
                table: "transactions",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "idx_type_status",
                table: "transactions",
                columns: new[] { "TransactionType", "TransactionStatus" });

            migrationBuilder.CreateIndex(
                name: "UserId6",
                table: "userprofiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_type",
                table: "users",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "AppliedBy",
                table: "usersanctions",
                column: "AppliedBy");

            migrationBuilder.CreateIndex(
                name: "idx_user_active",
                table: "usersanctions",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "UserId7",
                table: "userwallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_current",
                table: "workexperience",
                columns: new[] { "UserId", "IsCurrent" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adminactivitylogs");

            migrationBuilder.DropTable(
                name: "contentreports");

            migrationBuilder.DropTable(
                name: "contractsignatures");

            migrationBuilder.DropTable(
                name: "deliverablefiles");

            migrationBuilder.DropTable(
                name: "disputemessages");

            migrationBuilder.DropTable(
                name: "freelancerprofiles");

            migrationBuilder.DropTable(
                name: "freelancerskills");

            migrationBuilder.DropTable(
                name: "identityverifications");

            migrationBuilder.DropTable(
                name: "messageattachments");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "platformcommissions");

            migrationBuilder.DropTable(
                name: "portfoliofiles");

            migrationBuilder.DropTable(
                name: "projectactivitylog");

            migrationBuilder.DropTable(
                name: "projectapplications");

            migrationBuilder.DropTable(
                name: "projectskills");

            migrationBuilder.DropTable(
                name: "proposalcomments");

            migrationBuilder.DropTable(
                name: "proposalcostbreakdown");

            migrationBuilder.DropTable(
                name: "proposaldeliverables");

            migrationBuilder.DropTable(
                name: "proposaltimeline");

            migrationBuilder.DropTable(
                name: "reviewresponses");

            migrationBuilder.DropTable(
                name: "systemsettings");

            migrationBuilder.DropTable(
                name: "ticketresponses");

            migrationBuilder.DropTable(
                name: "userprofiles");

            migrationBuilder.DropTable(
                name: "usersanctions");

            migrationBuilder.DropTable(
                name: "userwallets");

            migrationBuilder.DropTable(
                name: "workexperience");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "projectdeliverables");

            migrationBuilder.DropTable(
                name: "disputes");

            migrationBuilder.DropTable(
                name: "projectmessages");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "portfolioitems");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "supporttickets");

            migrationBuilder.DropTable(
                name: "proposals");

            migrationBuilder.DropTable(
                name: "escrowaccounts");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
