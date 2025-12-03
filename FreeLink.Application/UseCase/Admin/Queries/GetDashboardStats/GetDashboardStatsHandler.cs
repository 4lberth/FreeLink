using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreeLink.Application.UseCase.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, GetDashboardStatsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardStatsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetDashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetDashboardStatsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para acceder al dashboard"
                };
            }

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfToday = now.Date;

            // Obtener todas las entidades necesarias
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var projects = await _unitOfWork.Repository<Project>().GetAll();
            var transactions = await _unitOfWork.Repository<Transaction>().GetAll();
            var commissions = await _unitOfWork.Repository<Platformcommission>().GetAll();
            var verifications = await _unitOfWork.Repository<Identityverification>().GetAll();
            var reports = await _unitOfWork.Repository<Contentreport>().GetAll();
            var disputes = await _unitOfWork.Repository<Dispute>().GetAll();
            var tickets = await _unitOfWork.Repository<Supportticket>().GetAll();
            var escrowAccounts = await _unitOfWork.Repository<Escrowaccount>().GetAll();

            // Construir estadísticas
            var stats = new DashboardStatsDto
            {
                UserStats = new UserStatsDto
                {
                    TotalUsers = users.Count(),
                    ActiveUsers = users.Count(u => u.IsActive == true),
                    InactiveUsers = users.Count(u => u.IsActive == false),
                    ClientsCount = users.Count(u => u.UserType == "Cliente"),
                    FreelancersCount = users.Count(u => u.UserType == "Freelancer"),
                    AdminsCount = users.Count(u => u.UserType == "Administrador"),
                    VerifiedUsers = users.Count(u => u.IsVerified == true),
                    NewUsersThisMonth = users.Count(u => u.CreatedAt >= startOfMonth)
                },

                ProjectStats = new ProjectStatsDto
                {
                    TotalProjects = projects.Count(),
                    PublishedProjects = projects.Count(p => p.ProjectStatus == "Publicado"),
                    InProgressProjects = projects.Count(p => p.ProjectStatus == "En Proceso"),
                    CompletedProjects = projects.Count(p => p.ProjectStatus == "Completado"),
                    CancelledProjects = projects.Count(p => p.ProjectStatus == "Cancelado"),
                    ProjectsThisMonth = projects.Count(p => p.CreatedAt >= startOfMonth)
                },

                FinancialStats = new FinancialStatsDto
                {
                    TotalTransactionsAmount = transactions.Sum(t => t.Amount),
                    TransactionsThisMonth = transactions.Where(t => t.CreatedAt >= startOfMonth).Sum(t => t.Amount),
                    CommissionsEarned = commissions.Sum(c => c.Amount),
                    CommissionsThisMonth = commissions.Where(c => c.CreatedAt >= startOfMonth).Sum(c => c.Amount),
                    EscrowBalance = escrowAccounts.Sum(e => e.TotalAmount),
                    TotalTransactions = transactions.Count()
                },

                PendingItems = new PendingItemsDto
                {
                    PendingVerifications = verifications.Count(v => v.VerificationStatus == "Pendiente"),
                    PendingReports = reports.Count(r => r.ReportStatus == "Pendiente"),
                    PendingDisputes = disputes.Count(d => d.DisputeStatus == "Abierta" || d.DisputeStatus == "En Revisión"),
                    OpenTickets = tickets.Count(t => t.TicketStatus == "Abierto" || t.TicketStatus == "En Progreso")
                },

                RecentActivity = new RecentActivityDto
                {
                    TransactionsToday = transactions.Count(t => t.CreatedAt >= startOfToday),
                    NewProjectsToday = projects.Count(p => p.CreatedAt >= startOfToday),
                    NewUsersToday = users.Count(u => u.CreatedAt >= startOfToday),
                    ReportsToday = reports.Count(r => r.CreatedAt >= startOfToday)
                }
            };

            return new GetDashboardStatsResponse
            {
                Success = true,
                Message = "Estadísticas obtenidas exitosamente",
                Data = stats
            };
        }
        catch (Exception ex)
        {
            return new GetDashboardStatsResponse
            {
                Success = false,
                Message = $"Error al obtener estadísticas: {ex.Message}"
            };
        }
    }
}
