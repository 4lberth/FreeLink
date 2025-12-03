using System.ComponentModel.DataAnnotations;

namespace FreeLink.Application.UseCase.Reports.DTOs;

public class CreateReportRequest
{
    /// <summary>
    /// ID del usuario reportado (opcional, si el reporte es sobre un usuario)
    /// </summary>
    public int? ReportedUserId { get; set; }

    /// <summary>
    /// ID del proyecto reportado (opcional, si el reporte es sobre un proyecto)
    /// </summary>
    public int? ReportedProjectId { get; set; }

    /// <summary>
    /// ID del mensaje reportado (opcional, si el reporte es sobre un mensaje)
    /// </summary>
    public int? ReportedMessageId { get; set; }

    /// <summary>
    /// Razón del reporte (obligatorio)
    /// Ejemplos: "Contenido inapropiado", "Spam", "Acoso", "Fraude", "Otro"
    /// </summary>
    [Required(ErrorMessage = "La razón del reporte es obligatoria")]
    [MaxLength(255, ErrorMessage = "La razón del reporte no puede superar 255 caracteres")]
    public string ReportReason { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del reporte (opcional)
    /// </summary>
    [MaxLength(1000, ErrorMessage = "La descripción no puede superar 1000 caracteres")]
    public string? ReportDescription { get; set; }
}
