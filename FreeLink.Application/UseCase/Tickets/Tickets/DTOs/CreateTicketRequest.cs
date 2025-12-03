using System.ComponentModel.DataAnnotations;

namespace FreeLink.Application.UseCase.Tickets.DTOs;

public class CreateTicketRequest
{
    /// <summary>
    /// Asunto del ticket (obligatorio)
    /// </summary>
    [Required(ErrorMessage = "El asunto es obligatorio")]
    [MaxLength(200, ErrorMessage = "El asunto no puede superar 200 caracteres")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del problema (obligatorio)
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MaxLength(2000, ErrorMessage = "La descripción no puede superar 2000 caracteres")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Prioridad del ticket (opcional)
    /// Valores: "Baja", "Media", "Alta"
    /// </summary>
    [MaxLength(50, ErrorMessage = "La prioridad no puede superar 50 caracteres")]
    public string? Priority { get; set; }
}
