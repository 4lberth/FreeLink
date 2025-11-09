using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeLink.Application.UseCase.Proposal;

namespace FreeLink.Application.Services
{
    public class InMemoryProposalService : IProposalService
    {
        private readonly ConcurrentDictionary<Guid, List<ProposalDto>> _store = new();

        public Task<ProposalDto> CreateAsync(ProposalCreateDto dto)
        {
            var id = Guid.NewGuid();
            var list = _store.GetOrAdd(dto.ProjectId, _ => new List<ProposalDto>());

            var version = list.Any() ? list.Max(p => p.Version) + 1 : 1;
            var proposal = new ProposalDto
            {
                Id = id,
                ProjectId = dto.ProjectId,
                Version = version,
                Title = dto.Title,
                Description = dto.Description,
                Cost = dto.Cost,
                CreatedAt = DateTime.UtcNow,
                Status = "Draft"
            };

            list.Add(proposal);
            return Task.FromResult(proposal);
        }

        public Task<ProposalDto?> GetByIdAsync(Guid id)
        {
            foreach (var kv in _store.Values)
            {
                var found = kv.FirstOrDefault(p => p.Id == id);
                if (found != null) return Task.FromResult<ProposalDto?>(found);
            }
            return Task.FromResult<ProposalDto?>(null);
        }

        public Task<IEnumerable<ProposalDto>> GetAllAsync()
        {
            var all = _store.Values.SelectMany(v => v).OrderByDescending(p => p.CreatedAt);
            return Task.FromResult<IEnumerable<ProposalDto>>(all);
        }
    }
}