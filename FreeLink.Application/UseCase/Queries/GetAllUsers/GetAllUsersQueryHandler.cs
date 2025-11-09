using AutoMapper;
using FreeLink.Domain.Entities; 
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FreeLink.Domain.Ports; 


namespace FreeLink.Application.UseCase.Admin.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<AdminUserDto>>
{
    private readonly IRepository<FreeLink.Domain.Entities.User> _userRepository; 
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IRepository<FreeLink.Domain.Entities.User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AdminUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAll(); // <-- ¡Arreglado!

        // NOTA: Para que esto funcione, debes añadir el mapeo en:
        // FreeLink.Application/Mappings/MappingProfile.cs
        // Dentro del constructor de MappingProfile, añade:
        // CreateMap<User, AdminUserDto>(); 
        // (Puedes mapear el nombre después si quieres)

        return _mapper.Map<IEnumerable<AdminUserDto>>(users);
    }
}