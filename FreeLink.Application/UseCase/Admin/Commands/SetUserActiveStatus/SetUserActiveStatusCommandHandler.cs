using FreeLink.Domain.Ports; // Para IRepository y IUnitOfWork
using MediatR;
using System.Threading;
using System.Threading.Tasks;

// ¡Importante! Usamos el nombre completo para evitar conflictos
using User = FreeLink.Domain.Entities.User; 

namespace FreeLink.Application.UseCase.Admin.Commands.SetUserActiveStatus;

public class SetUserActiveStatusCommandHandler : IRequestHandler<SetUserActiveStatusCommand, bool>
{
    private readonly IRepository<FreeLink.Domain.Entities.User> _userRepository;
    private readonly IUnitOfWork _unitOfWork; // Para guardar los cambios

    public SetUserActiveStatusCommandHandler(IRepository<FreeLink.Domain.Entities.User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar al usuario
        var user = await _userRepository.GetById(request.UserId);

        if (user == null)
        {
            return false; // No se encontró al usuario
        }

        // 2. Aplicar el cambio
        user.IsActive = request.IsActive; // Tu User.cs tiene esta propiedad

        // 3. Guardar en la Base de Datos
        _userRepository.Update(user); // Le decimos a EF que este usuario cambió
        await _unitOfWork.Complete(); 

        return true;
    }
}