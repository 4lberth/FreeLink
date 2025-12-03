// using ClosedXML.Excel;
// using FreeLink.Domain.Entities;
// using FreeLink.Domain.Ports;
// using Microsoft.EntityFrameworkCore;
//
// namespace FreeLink.Infrastructure.Services;
//
// public class ExcelReportService : IExcelReportService
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     public ExcelReportService(IUnitOfWork unitOfWork)
//     {
//         _unitOfWork = unitOfWork;
//     }
//
//     public async Task<byte[]> GenerateUsersReportAsync(DateTime? startDate = null, DateTime? endDate = null)
//     {
//         var query = await _unitOfWork.Repository<User>().GetAllAsync();
//         var users = query.AsQueryable();
//
//         if (startDate.HasValue)
//             users = users.Where(u => u.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             users = users.Where(u => u.CreatedAt <= endDate.Value);
//
//         var usersList = users.OrderByDescending(u => u.CreatedAt).ToList();
//
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Usuarios");
//
//         // Headers
//         worksheet.Cell(1, 1).Value = "ID";
//         worksheet.Cell(1, 2).Value = "Email";
//         worksheet.Cell(1, 3).Value = "Tipo de Usuario";
//         worksheet.Cell(1, 4).Value = "Username";
//         worksheet.Cell(1, 5).Value = "Verificado";
//         worksheet.Cell(1, 6).Value = "Activo";
//         worksheet.Cell(1, 7).Value = "Fecha de Registro";
//         worksheet.Cell(1, 8).Value = "Última Actualización";
//
//         // Style headers
//         var headerRow = worksheet.Range(1, 1, 1, 8);
//         headerRow.Style.Font.Bold = true;
//         headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//         headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//
//         // Data
//         int row = 2;
//         foreach (var user in usersList)
//         {
//             worksheet.Cell(row, 1).Value = user.UserId;
//             worksheet.Cell(row, 2).Value = user.Email;
//             worksheet.Cell(row, 3).Value = user.UserType;
//             worksheet.Cell(row, 4).Value = user.Username;
//             worksheet.Cell(row, 5).Value = user.IsVerified ? "Sí" : "No";
//             worksheet.Cell(row, 6).Value = user.IsActive ? "Sí" : "No";
//             worksheet.Cell(row, 7).Value = user.CreatedAt;
//             worksheet.Cell(row, 8).Value = user.UpdatedAt;
//             row++;
//         }
//
//         // Auto-fit columns
//         worksheet.Columns().AdjustToContents();
//
//         // Summary
//         worksheet.Cell(row + 1, 1).Value = "Total de Usuarios:";
//         worksheet.Cell(row + 1, 2).Value = usersList.Count;
//         worksheet.Cell(row + 1, 1).Style.Font.Bold = true;
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
//
//     public async Task<byte[]> GenerateProjectsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null)
//     {
//         var query = await _unitOfWork.Repository<Project>().GetAllAsync();
//         var projects = query.AsQueryable();
//
//         if (startDate.HasValue)
//             projects = projects.Where(p => p.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             projects = projects.Where(p => p.CreatedAt <= endDate.Value);
//
//         if (!string.IsNullOrEmpty(status))
//             projects = projects.Where(p => p.ProjectStatus == status);
//
//         var projectsList = projects.OrderByDescending(p => p.CreatedAt).ToList();
//
//         // Get related data
//         var clientIds = projectsList.Select(p => p.ClientId).Distinct().ToList();
//         var clients = await _unitOfWork.Repository<User>().GetAsync(u => clientIds.Contains(u.UserId));
//         var clientDict = clients.ToDictionary(c => c.UserId, c => c.Email);
//
//         var freelancerIds = projectsList.Where(p => p.AssignedFreelancerId.HasValue)
//             .Select(p => p.AssignedFreelancerId!.Value).Distinct().ToList();
//         var freelancers = await _unitOfWork.Repository<User>().GetAsync(u => freelancerIds.Contains(u.UserId));
//         var freelancerDict = freelancers.ToDictionary(f => f.UserId, f => f.Email);
//
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Proyectos");
//
//         // Headers
//         worksheet.Cell(1, 1).Value = "ID";
//         worksheet.Cell(1, 2).Value = "Título";
//         worksheet.Cell(1, 3).Value = "Cliente";
//         worksheet.Cell(1, 4).Value = "Freelancer Asignado";
//         worksheet.Cell(1, 5).Value = "Presupuesto";
//         worksheet.Cell(1, 6).Value = "Estado";
//         worksheet.Cell(1, 7).Value = "Fecha Límite";
//         worksheet.Cell(1, 8).Value = "Fecha de Creación";
//
//         // Style headers
//         var headerRow = worksheet.Range(1, 1, 1, 8);
//         headerRow.Style.Font.Bold = true;
//         headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//         headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//
//         // Data
//         int row = 2;
//         decimal totalBudget = 0;
//         foreach (var project in projectsList)
//         {
//             worksheet.Cell(row, 1).Value = project.ProjectId;
//             worksheet.Cell(row, 2).Value = project.Title;
//             worksheet.Cell(row, 3).Value = clientDict.GetValueOrDefault(project.ClientId, "N/A");
//             worksheet.Cell(row, 4).Value = project.AssignedFreelancerId.HasValue
//                 ? freelancerDict.GetValueOrDefault(project.AssignedFreelancerId.Value, "N/A")
//                 : "No asignado";
//             worksheet.Cell(row, 5).Value = project.Budget;
//             worksheet.Cell(row, 6).Value = project.ProjectStatus;
//             worksheet.Cell(row, 7).Value = project.DeadlineDate?.ToString("yyyy-MM-dd") ?? "N/A";
//             worksheet.Cell(row, 8).Value = project.CreatedAt;
//
//             totalBudget += project.Budget ?? 0;
//             row++;
//         }
//
//         // Auto-fit columns
//         worksheet.Columns().AdjustToContents();
//
//         // Summary
//         worksheet.Cell(row + 1, 1).Value = "Total de Proyectos:";
//         worksheet.Cell(row + 1, 2).Value = projectsList.Count;
//         worksheet.Cell(row + 2, 1).Value = "Presupuesto Total:";
//         worksheet.Cell(row + 2, 2).Value = totalBudget;
//         worksheet.Cell(row + 2, 2).Style.NumberFormat.Format = "$#,##0.00";
//
//         worksheet.Range(row + 1, 1, row + 2, 1).Style.Font.Bold = true;
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
//
//     public async Task<byte[]> GenerateTransactionsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? transactionType = null)
//     {
//         var query = await _unitOfWork.Repository<Transaction>().GetAllAsync();
//         var transactions = query.AsQueryable();
//
//         if (startDate.HasValue)
//             transactions = transactions.Where(t => t.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             transactions = transactions.Where(t => t.CreatedAt <= endDate.Value);
//
//         if (!string.IsNullOrEmpty(transactionType))
//             transactions = transactions.Where(t => t.TransactionType == transactionType);
//
//         var transactionsList = transactions.OrderByDescending(t => t.CreatedAt).ToList();
//
//         // Get user data
//         var userIds = transactionsList
//             .Select(t => t.FromUserId)
//             .Union(transactionsList.Select(t => t.ToUserId))
//             .Where(id => id.HasValue)
//             .Select(id => id!.Value)
//             .Distinct()
//             .ToList();
//
//         var users = await _unitOfWork.Repository<User>().GetAsync(u => userIds.Contains(u.UserId));
//         var userDict = users.ToDictionary(u => u.UserId, u => u.Email);
//
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Transacciones");
//
//         // Headers
//         worksheet.Cell(1, 1).Value = "ID";
//         worksheet.Cell(1, 2).Value = "De Usuario";
//         worksheet.Cell(1, 3).Value = "A Usuario";
//         worksheet.Cell(1, 4).Value = "Monto";
//         worksheet.Cell(1, 5).Value = "Tipo";
//         worksheet.Cell(1, 6).Value = "Estado";
//         worksheet.Cell(1, 7).Value = "Fecha";
//
//         // Style headers
//         var headerRow = worksheet.Range(1, 1, 1, 7);
//         headerRow.Style.Font.Bold = true;
//         headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//         headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//
//         // Data
//         int row = 2;
//         decimal totalAmount = 0;
//         foreach (var transaction in transactionsList)
//         {
//             worksheet.Cell(row, 1).Value = transaction.TransactionId;
//             worksheet.Cell(row, 2).Value = transaction.FromUserId.HasValue
//                 ? userDict.GetValueOrDefault(transaction.FromUserId.Value, "N/A")
//                 : "Sistema";
//             worksheet.Cell(row, 3).Value = transaction.ToUserId.HasValue
//                 ? userDict.GetValueOrDefault(transaction.ToUserId.Value, "N/A")
//                 : "Sistema";
//             worksheet.Cell(row, 4).Value = transaction.Amount;
//             worksheet.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";
//             worksheet.Cell(row, 5).Value = transaction.TransactionType;
//             worksheet.Cell(row, 6).Value = transaction.TransactionStatus;
//             worksheet.Cell(row, 7).Value = transaction.CreatedAt;
//
//             if (transaction.TransactionStatus == "Completada")
//                 totalAmount += transaction.Amount;
//
//             row++;
//         }
//
//         // Auto-fit columns
//         worksheet.Columns().AdjustToContents();
//
//         // Summary
//         worksheet.Cell(row + 1, 1).Value = "Total de Transacciones:";
//         worksheet.Cell(row + 1, 2).Value = transactionsList.Count;
//         worksheet.Cell(row + 2, 1).Value = "Monto Total Procesado:";
//         worksheet.Cell(row + 2, 2).Value = totalAmount;
//         worksheet.Cell(row + 2, 2).Style.NumberFormat.Format = "$#,##0.00";
//
//         worksheet.Range(row + 1, 1, row + 2, 1).Style.Font.Bold = true;
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
//
//     public async Task<byte[]> GenerateVerificationsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null)
//     {
//         var query = await _unitOfWork.Repository<Identityverification>().GetAllAsync();
//         var verifications = query.AsQueryable();
//
//         if (startDate.HasValue)
//             verifications = verifications.Where(v => v.SubmittedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             verifications = verifications.Where(v => v.SubmittedAt <= endDate.Value);
//
//         if (!string.IsNullOrEmpty(status))
//             verifications = verifications.Where(v => v.VerificationStatus == status);
//
//         var verificationsList = verifications.OrderByDescending(v => v.SubmittedAt).ToList();
//
//         // Get user data
//         var userIds = verificationsList.Select(v => v.UserId).Distinct().ToList();
//         var users = await _unitOfWork.Repository<User>().GetAsync(u => userIds.Contains(u.UserId));
//         var userDict = users.ToDictionary(u => u.UserId, u => u.Email);
//
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Verificaciones");
//
//         // Headers
//         worksheet.Cell(1, 1).Value = "ID";
//         worksheet.Cell(1, 2).Value = "Usuario";
//         worksheet.Cell(1, 3).Value = "Tipo de Documento";
//         worksheet.Cell(1, 4).Value = "Número de Documento";
//         worksheet.Cell(1, 5).Value = "Estado";
//         worksheet.Cell(1, 6).Value = "Fecha de Envío";
//         worksheet.Cell(1, 7).Value = "Fecha de Revisión";
//
//         // Style headers
//         var headerRow = worksheet.Range(1, 1, 1, 7);
//         headerRow.Style.Font.Bold = true;
//         headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//         headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//
//         // Data
//         int row = 2;
//         int approved = 0, rejected = 0, pending = 0;
//         foreach (var verification in verificationsList)
//         {
//             worksheet.Cell(row, 1).Value = verification.VerificationId;
//             worksheet.Cell(row, 2).Value = userDict.GetValueOrDefault(verification.UserId, "N/A");
//             worksheet.Cell(row, 3).Value = verification.DocumentType;
//             worksheet.Cell(row, 4).Value = verification.DocumentNumber;
//             worksheet.Cell(row, 5).Value = verification.VerificationStatus;
//             worksheet.Cell(row, 6).Value = verification.SubmittedAt;
//             worksheet.Cell(row, 7).Value = verification.ReviewedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
//
//             if (verification.VerificationStatus == "Aprobada") approved++;
//             else if (verification.VerificationStatus == "Rechazada") rejected++;
//             else if (verification.VerificationStatus == "Pendiente") pending++;
//
//             row++;
//         }
//
//         // Auto-fit columns
//         worksheet.Columns().AdjustToContents();
//
//         // Summary
//         worksheet.Cell(row + 1, 1).Value = "Total de Verificaciones:";
//         worksheet.Cell(row + 1, 2).Value = verificationsList.Count;
//         worksheet.Cell(row + 2, 1).Value = "Aprobadas:";
//         worksheet.Cell(row + 2, 2).Value = approved;
//         worksheet.Cell(row + 3, 1).Value = "Rechazadas:";
//         worksheet.Cell(row + 3, 2).Value = rejected;
//         worksheet.Cell(row + 4, 1).Value = "Pendientes:";
//         worksheet.Cell(row + 4, 2).Value = pending;
//
//         worksheet.Range(row + 1, 1, row + 4, 1).Style.Font.Bold = true;
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
//
//     public async Task<byte[]> GenerateSupportTicketsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null)
//     {
//         var query = await _unitOfWork.Repository<Supportticket>().GetAllAsync();
//         var tickets = query.AsQueryable();
//
//         if (startDate.HasValue)
//             tickets = tickets.Where(t => t.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             tickets = tickets.Where(t => t.CreatedAt <= endDate.Value);
//
//         if (!string.IsNullOrEmpty(status))
//             tickets = tickets.Where(t => t.TicketStatus == status);
//
//         var ticketsList = tickets.OrderByDescending(t => t.CreatedAt).ToList();
//
//         // Get user data
//         var userIds = ticketsList.Select(t => t.UserId).Distinct().ToList();
//         var users = await _unitOfWork.Repository<User>().GetAsync(u => userIds.Contains(u.UserId));
//         var userDict = users.ToDictionary(u => u.UserId, u => u.Email);
//
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Tickets");
//
//         // Headers
//         worksheet.Cell(1, 1).Value = "ID";
//         worksheet.Cell(1, 2).Value = "Usuario";
//         worksheet.Cell(1, 3).Value = "Asunto";
//         worksheet.Cell(1, 4).Value = "Prioridad";
//         worksheet.Cell(1, 5).Value = "Estado";
//         worksheet.Cell(1, 6).Value = "Fecha de Creación";
//         worksheet.Cell(1, 7).Value = "Fecha de Resolución";
//
//         // Style headers
//         var headerRow = worksheet.Range(1, 1, 1, 7);
//         headerRow.Style.Font.Bold = true;
//         headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//         headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//
//         // Data
//         int row = 2;
//         int open = 0, inProgress = 0, resolved = 0, closed = 0;
//         foreach (var ticket in ticketsList)
//         {
//             worksheet.Cell(row, 1).Value = ticket.TicketId;
//             worksheet.Cell(row, 2).Value = userDict.GetValueOrDefault(ticket.UserId, "N/A");
//             worksheet.Cell(row, 3).Value = ticket.Subject;
//             worksheet.Cell(row, 4).Value = ticket.Priority;
//             worksheet.Cell(row, 5).Value = ticket.TicketStatus;
//             worksheet.Cell(row, 6).Value = ticket.CreatedAt;
//             worksheet.Cell(row, 7).Value = ticket.ResolvedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
//
//             switch (ticket.TicketStatus)
//             {
//                 case "Abierto": open++; break;
//                 case "En Proceso": inProgress++; break;
//                 case "Resuelto": resolved++; break;
//                 case "Cerrado": closed++; break;
//             }
//
//             row++;
//         }
//
//         // Auto-fit columns
//         worksheet.Columns().AdjustToContents();
//
//         // Summary
//         worksheet.Cell(row + 1, 1).Value = "Total de Tickets:";
//         worksheet.Cell(row + 1, 2).Value = ticketsList.Count;
//         worksheet.Cell(row + 2, 1).Value = "Abiertos:";
//         worksheet.Cell(row + 2, 2).Value = open;
//         worksheet.Cell(row + 3, 1).Value = "En Proceso:";
//         worksheet.Cell(row + 3, 2).Value = inProgress;
//         worksheet.Cell(row + 4, 1).Value = "Resueltos:";
//         worksheet.Cell(row + 4, 2).Value = resolved;
//         worksheet.Cell(row + 5, 1).Value = "Cerrados:";
//         worksheet.Cell(row + 5, 2).Value = closed;
//
//         worksheet.Range(row + 1, 1, row + 5, 1).Style.Font.Bold = true;
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
//
//     public async Task<byte[]> GenerateContentReportsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null)
//     {
//         var query = await _unitOfWork.Repository<Contentreport>().GetAllAsync();
//         var reports = query.AsQueryable();
//
//         if (startDate.HasValue)
//             reports = reports.Where(r => r.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             reports = reports.Where(r => r.CreatedAt <= endDate.Value);
//
//         if (!string.IsNullOrEmpty(status))
//             reports = reports.Where(r => r.ReportStatus == status);
//
//         var reportsList = reports.OrderByDescending(r => r.CreatedAt).ToList();
//
//         // Get user data
//         var userIds = reportsList.Select(r => r.ReporterId)
//             .Union(reportsList.Where(r => r.ReportedUserId.HasValue).Select(r => r.ReportedUserId!.Value))
//             .Distinct()
//             .ToList();
//         var users = await _unitOfWork.Repository<User>().GetAsync(u => userIds.Contains(u.UserId));
//         var userDict = users.ToDictionary(u => u.UserId, u => u.Email);
//
//         using var workbook = new XLWorkbook();
//         var worksheet = workbook.Worksheets.Add("Reportes");
//
//         // Headers
//         worksheet.Cell(1, 1).Value = "ID";
//         worksheet.Cell(1, 2).Value = "Reportero";
//         worksheet.Cell(1, 3).Value = "Reportado";
//         worksheet.Cell(1, 4).Value = "Razón";
//         worksheet.Cell(1, 5).Value = "Estado";
//         worksheet.Cell(1, 6).Value = "Fecha";
//
//         // Style headers
//         var headerRow = worksheet.Range(1, 1, 1, 6);
//         headerRow.Style.Font.Bold = true;
//         headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//         headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//
//         // Data
//         int row = 2;
//         int pending = 0, inReview = 0, resolved = 0, rejected = 0;
//         foreach (var report in reportsList)
//         {
//             worksheet.Cell(row, 1).Value = report.ReportId;
//             worksheet.Cell(row, 2).Value = userDict.GetValueOrDefault(report.ReporterId, "N/A");
//
//             string reported = "N/A";
//             if (report.ReportedUserId.HasValue)
//                 reported = $"Usuario: {userDict.GetValueOrDefault(report.ReportedUserId.Value, "N/A")}";
//             else if (report.ReportedProjectId.HasValue)
//                 reported = $"Proyecto ID: {report.ReportedProjectId.Value}";
//             else if (report.ReportedMessageId.HasValue)
//                 reported = $"Mensaje ID: {report.ReportedMessageId.Value}";
//
//             worksheet.Cell(row, 3).Value = reported;
//             worksheet.Cell(row, 4).Value = report.ReportReason;
//             worksheet.Cell(row, 5).Value = report.ReportStatus;
//             worksheet.Cell(row, 6).Value = report.CreatedAt;
//
//             switch (report.ReportStatus)
//             {
//                 case "Pendiente": pending++; break;
//                 case "En Revisión": inReview++; break;
//                 case "Resuelto": resolved++; break;
//                 case "Rechazado": rejected++; break;
//             }
//
//             row++;
//         }
//
//         // Auto-fit columns
//         worksheet.Columns().AdjustToContents();
//
//         // Summary
//         worksheet.Cell(row + 1, 1).Value = "Total de Reportes:";
//         worksheet.Cell(row + 1, 2).Value = reportsList.Count;
//         worksheet.Cell(row + 2, 1).Value = "Pendientes:";
//         worksheet.Cell(row + 2, 2).Value = pending;
//         worksheet.Cell(row + 3, 1).Value = "En Revisión:";
//         worksheet.Cell(row + 3, 2).Value = inReview;
//         worksheet.Cell(row + 4, 1).Value = "Resueltos:";
//         worksheet.Cell(row + 4, 2).Value = resolved;
//         worksheet.Cell(row + 5, 1).Value = "Rechazados:";
//         worksheet.Cell(row + 5, 2).Value = rejected;
//
//         worksheet.Range(row + 1, 1, row + 5, 1).Style.Font.Bold = true;
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
//
//     public async Task<byte[]> GenerateFinancialReportAsync(DateTime? startDate = null, DateTime? endDate = null)
//     {
//         var query = await _unitOfWork.Repository<Transaction>().GetAllAsync();
//         var transactions = query.AsQueryable();
//
//         if (startDate.HasValue)
//             transactions = transactions.Where(t => t.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             transactions = transactions.Where(t => t.CreatedAt <= endDate.Value);
//
//         var transactionsList = transactions.Where(t => t.TransactionStatus == "Completada").ToList();
//
//         // Get commission data
//         var commissionQuery = await _unitOfWork.Repository<Platformcommission>().GetAllAsync();
//         var commissions = commissionQuery.AsQueryable();
//
//         if (startDate.HasValue)
//             commissions = commissions.Where(c => c.CreatedAt >= startDate.Value);
//
//         if (endDate.HasValue)
//             commissions = commissions.Where(c => c.CreatedAt <= endDate.Value);
//
//         var commissionsList = commissions.ToList();
//
//         using var workbook = new XLWorkbook();
//
//         // Summary Sheet
//         var summarySheet = workbook.Worksheets.Add("Resumen Financiero");
//
//         summarySheet.Cell(1, 1).Value = "REPORTE FINANCIERO";
//         summarySheet.Cell(1, 1).Style.Font.Bold = true;
//         summarySheet.Cell(1, 1).Style.Font.FontSize = 16;
//
//         summarySheet.Cell(3, 1).Value = "Período:";
//         summarySheet.Cell(3, 2).Value = $"{startDate?.ToString("yyyy-MM-dd") ?? "Inicio"} - {endDate?.ToString("yyyy-MM-dd") ?? "Actual"}";
//
//         var deposits = transactionsList.Where(t => t.TransactionType == "Depósito").Sum(t => t.Amount);
//         var releases = transactionsList.Where(t => t.TransactionType == "Liberación").Sum(t => t.Amount);
//         var withdrawals = transactionsList.Where(t => t.TransactionType == "Retiro").Sum(t => t.Amount);
//         var totalCommissions = commissionsList.Sum(c => c.Amount);
//
//         summarySheet.Cell(5, 1).Value = "TRANSACCIONES";
//         summarySheet.Cell(5, 1).Style.Font.Bold = true;
//         summarySheet.Cell(6, 1).Value = "Total Depósitos:";
//         summarySheet.Cell(6, 2).Value = deposits;
//         summarySheet.Cell(6, 2).Style.NumberFormat.Format = "$#,##0.00";
//         summarySheet.Cell(7, 1).Value = "Total Liberaciones:";
//         summarySheet.Cell(7, 2).Value = releases;
//         summarySheet.Cell(7, 2).Style.NumberFormat.Format = "$#,##0.00";
//         summarySheet.Cell(8, 1).Value = "Total Retiros:";
//         summarySheet.Cell(8, 2).Value = withdrawals;
//         summarySheet.Cell(8, 2).Style.NumberFormat.Format = "$#,##0.00";
//
//         summarySheet.Cell(10, 1).Value = "COMISIONES DE PLATAFORMA";
//         summarySheet.Cell(10, 1).Style.Font.Bold = true;
//         summarySheet.Cell(11, 1).Value = "Total Comisiones:";
//         summarySheet.Cell(11, 2).Value = totalCommissions;
//         summarySheet.Cell(11, 2).Style.NumberFormat.Format = "$#,##0.00";
//         summarySheet.Cell(11, 2).Style.Font.Bold = true;
//         summarySheet.Cell(11, 2).Style.Fill.BackgroundColor = XLColor.LightGreen;
//
//         summarySheet.Cell(13, 1).Value = "Número de Transacciones:";
//         summarySheet.Cell(13, 2).Value = transactionsList.Count;
//         summarySheet.Cell(14, 1).Value = "Número de Comisiones:";
//         summarySheet.Cell(14, 2).Value = commissionsList.Count;
//
//         summarySheet.Columns().AdjustToContents();
//
//         // Transactions Detail Sheet
//         var transSheet = workbook.Worksheets.Add("Detalle Transacciones");
//         transSheet.Cell(1, 1).Value = "Tipo";
//         transSheet.Cell(1, 2).Value = "Monto";
//         transSheet.Cell(1, 3).Value = "Fecha";
//
//         var transHeaderRow = transSheet.Range(1, 1, 1, 3);
//         transHeaderRow.Style.Font.Bold = true;
//         transHeaderRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
//
//         int transRow = 2;
//         foreach (var trans in transactionsList.OrderBy(t => t.CreatedAt))
//         {
//             transSheet.Cell(transRow, 1).Value = trans.TransactionType;
//             transSheet.Cell(transRow, 2).Value = trans.Amount;
//             transSheet.Cell(transRow, 2).Style.NumberFormat.Format = "$#,##0.00";
//             transSheet.Cell(transRow, 3).Value = trans.CreatedAt;
//             transRow++;
//         }
//
//         transSheet.Columns().AdjustToContents();
//
//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }
// }
