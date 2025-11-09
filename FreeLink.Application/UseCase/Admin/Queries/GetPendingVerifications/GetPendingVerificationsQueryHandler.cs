using AutoMapper;
using FreeLink.Domain.Enums;
using FreeLink.Domain.Ports;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;

public class GetPendingVerificationsQueryHandler : IRequestHandler<GetPendingVerificationsQuery, IEnumerable<PendingVerificationDto>>
{
    // Usamos el nombre completo para la entidad
    private readonly IRepository<FreeLink.Domain.Entities.Identityverification> _verificationRepository;
    private readonly IMapper _mapper;

    public GetPendingVerificationsQueryHandler(IRepository<FreeLink.Domain.Entities.Identityverification> verificationRepository, IMapper mapper)
    {
        _verificationRepository = verificationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PendingVerificationDto>> Handle(GetPendingVerificationsQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscamos solo las pendientes
        var verifications = await _verificationRepository.GetAsync(v => v.Status == VerificationStatus.Pending);

        // 2. Mapeamos al DTO
        // (Necesitarás configurar este mapeo en MappingProfile.cs)
        return _mapper.Map<IEnumerable<PendingVerificationDto>>(verifications);
    }
}