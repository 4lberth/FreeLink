using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FreeLink.Application.UseCase.User.DTOs;

public class SubmitIdentityVerificationRequest
{
    [Required(ErrorMessage = "El tipo de documento es obligatorio")]
    public string DocumentType { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de documento es obligatorio")]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La foto del frente del documento es obligatoria")]
    public IFormFile DocumentFront { get; set; } = null!;

    [Required(ErrorMessage = "La foto del reverso del documento es obligatoria")]
    public IFormFile DocumentBack { get; set; } = null!;

    [Required(ErrorMessage = "La selfie es obligatoria")]
    public IFormFile Selfie { get; set; } = null!;
}
