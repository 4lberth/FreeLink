using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.SetUserActiveStatus;

// Usamos bool como respuesta para saber si funcionó
public class SetUserActiveStatusCommand : IRequest<bool> 
{
    public int UserId { get; set; } // El 'int' de tu User.cs
    public bool IsActive { get; set; }
}