using FreeLink.Domain.Ports;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FreeLink.Application.UseCase.Admin.Commands.ChangeUserType;

public class ChangeUserTypeCommandHandler : IRequestHandler<ChangeUserTypeCommand, bool>
{

    private readonly IRepository<FreeLink.Domain.Entities.User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeUserTypeCommandHandler(IRepository<FreeLink.Domain.Entities.User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangeUserTypeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.UserId);

        if (user == null)
        {
            return false; 
        }

        // 1. Aplicar el cambio
        user.UserType = request.NewUserType; // Asignamos el nuevo string

        // 2. Guardar
        _userRepository.Update(user);
        await _unitOfWork.Complete(); // Usamos Complete() como descubrimos

        return true;
    }
}