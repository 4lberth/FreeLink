using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeLink.Application.UseCase.Proposal;

namespace FreeLink.Application.Services
{
    public interface IProposalService
    {
        Task<ProposalDto> CreateAsync(ProposalCreateDto dto);
        Task<ProposalDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProposalDto>> GetAllAsync();
    }
}