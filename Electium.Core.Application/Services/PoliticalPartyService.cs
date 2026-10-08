using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class PoliticalPartyService : GenericService<PoliticalPartyDto, PoliticalParty>, IPoliticalPartyService
    {
        private readonly IGenericRepository<PoliticalParty> _partyRepository;

        public PoliticalPartyService(IGenericRepository<PoliticalParty> partyRepository,
            IMapper mapper) : base(partyRepository, mapper)
        {
            _partyRepository = partyRepository;
        }

        public async Task<bool> IsAcronymTakenAsync(
            string acronym,
            int? excludePartyId = null)
        {
            return await _partyRepository.AnyAsync(p => p.Acronym == acronym &&
            (excludePartyId == null || p.Id != excludePartyId));
        }
    }
}
