using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ChangeUserType;

// Devuelve 'bool' (verdadero) si tuvo éxito
public class ChangeUserTypeCommand : IRequest<bool>
{
    public int UserId { get; set; }

    // Usamos 'string' porque así lo definimos en tu User.cs
    public string NewUserType { get; set; } 
}