namespace FreeLink.Domain.Ports;

/// <summary>
/// Servicio para generar reportes en formato Excel
/// </summary>
public interface IExcelReportService
{
    /// <summary>
    /// Genera un reporte de usuarios en Excel
    /// </summary>
    Task<byte[]> GenerateUsersReportAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Genera un reporte de proyectos en Excel
    /// </summary>
    Task<byte[]> GenerateProjectsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null);

    /// <summary>
    /// Genera un reporte de transacciones en Excel
    /// </summary>
    Task<byte[]> GenerateTransactionsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? transactionType = null);

    /// <summary>
    /// Genera un reporte de verificaciones de identidad en Excel
    /// </summary>
    Task<byte[]> GenerateVerificationsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null);

    /// <summary>
    /// Genera un reporte de tickets de soporte en Excel
    /// </summary>
    Task<byte[]> GenerateSupportTicketsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null);

    /// <summary>
    /// Genera un reporte de reportes de contenido en Excel
    /// </summary>
    Task<byte[]> GenerateContentReportsReportAsync(DateTime? startDate = null, DateTime? endDate = null, string? status = null);

    /// <summary>
    /// Genera un reporte financiero general en Excel
    /// </summary>
    Task<byte[]> GenerateFinancialReportAsync(DateTime? startDate = null, DateTime? endDate = null);
}
