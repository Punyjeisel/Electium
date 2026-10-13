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
    public class PartyLeaderService : GenericService<PartyLeaderDto, PartyLeader>, IPartyLeaderService
    {
        private readonly IGenericRepository<PartyLeader> _partyLeaderRepository;

        public PartyLeaderService(
            IGenericRepository<PartyLeader> partyLeaderRepository,
            IMapper mapper)
            : base(partyLeaderRepository, mapper)
        {
            _partyLeaderRepository = partyLeaderRepository;
        }

        public async Task<List<PartyLeaderDto>> GetAllWithIncludesAsync()
        {
            var leaders = await _partyLeaderRepository.GetAllAsync(
                null,
                "User",
                "PoliticalParty");

            return _mapper.Map<List<PartyLeaderDto>>(leaders);
        }

        // A leader can only belong to one party
        public async Task<bool> IsUserAlreadyAssignedAsync(int userId)
        {
            return await _partyLeaderRepository.AnyAsync(
                pl => pl.UserId == userId && pl.IsActive);
        }

        public async Task CreateAsync(CreatePartyLeaderDto dto)
        {
            if (await IsUserAlreadyAssignedAsync(dto.UserId))
                throw new InvalidOperationException(
                    "Este dirigente ya está relacionado con otro partido político.");

            var leader = _mapper.Map<PartyLeader>(dto);
            leader.IsActive = true;

            await _partyLeaderRepository.AddAsync(leader);
        }
    }
}
