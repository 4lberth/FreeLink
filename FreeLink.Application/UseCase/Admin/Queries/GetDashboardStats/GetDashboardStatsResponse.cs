namespace FreeLink.Application.UseCase.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DashboardStatsDto? Data { get; set; }
}

public class DashboardStatsDto
{
    // Estadísticas de Usuarios
    public UserStatsDto UserStats { get; set; } = new();

    // Estadísticas de Proyectos
    public ProjectStatsDto ProjectStats { get; set; } = new();

    // Estadísticas Financieras
    public FinancialStatsDto FinancialStats { get; set; } = new();

    // Items Pendientes de Revisión
    public PendingItemsDto PendingItems { get; set; } = new();

    // Actividad Reciente
    public RecentActivityDto RecentActivity { get; set; } = new();
}

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int ClientsCount { get; set; }
    public int FreelancersCount { get; set; }
    public int AdminsCount { get; set; }
    public int VerifiedUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
}

public class ProjectStatsDto
{
    public int TotalProjects { get; set; }
    public int PublishedProjects { get; set; }
    public int InProgressProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int CancelledProjects { get; set; }
    public int ProjectsThisMonth { get; set; }
}

public class FinancialStatsDto
{
    public decimal TotalTransactionsAmount { get; set; }
    public decimal TransactionsThisMonth { get; set; }
    public decimal CommissionsEarned { get; set; }
    public decimal CommissionsThisMonth { get; set; }
    public decimal EscrowBalance { get; set; }
    public int TotalTransactions { get; set; }
}

public class PendingItemsDto
{
    public int PendingVerifications { get; set; }
    public int PendingReports { get; set; }
    public int PendingDisputes { get; set; }
    public int OpenTickets { get; set; }
}

public class RecentActivityDto
{
    public int TransactionsToday { get; set; }
    public int NewProjectsToday { get; set; }
    public int NewUsersToday { get; set; }
    public int ReportsToday { get; set; }
}
