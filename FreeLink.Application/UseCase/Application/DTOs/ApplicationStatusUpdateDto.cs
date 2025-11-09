namespace FreeLink.Application.UseCase.Application.DTOs
{
    public class ApplicationStatusUpdateDto
    {
        // "Aceptada" o "Rechazada"
        public string NewStatus { get; set; } = string.Empty; 
    }
}