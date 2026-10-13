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
    public class CandidatePositionService : GenericService<CandidatePositionDto, CandidatePosition>, ICandidatePositionService
    {
        private readonly IGenericRepository<CandidatePosition> _candidatePositionRepository;

        public CandidatePositionService(
            IGenericRepository<CandidatePosition> candidatePositionRepository,
            IMapper mapper)
            : base(candidatePositionRepository, mapper)
        {
            _candidatePositionRepository = candidatePositionRepository;
        }

        public async Task<List<CandidatePositionDto>> GetByPartyAsync(int partyId)
        {
            var list = await _candidatePositionRepository.GetAllAsync(
                cp => cp.PoliticalPartyId == partyId,
                "Candidate",
                "ElectivePosition");

            return _mapper.Map<List<CandidatePositionDto>>(list);
        }

        // Rules from the document:
        // 1. A candidate cannot hold two positions in the same party.
        // 2. A party cannot have two candidates for the same position.
        public async Task CreateAsync(CreateCandidatePositionDto dto)
        {
            // Rule 1
            var candidateAlreadyAssigned = await _candidatePositionRepository.AnyAsync(
                cp => cp.CandidateId == dto.CandidateId
                   && cp.PoliticalPartyId == dto.PoliticalPartyId);

            if (candidateAlreadyAssigned)
                throw new InvalidOperationException(
                    "Este candidato ya está asignado a un puesto dentro del partido.");

            // Rule 2
            var positionAlreadyTaken = await _candidatePositionRepository.AnyAsync(
                cp => cp.ElectivePositionId == dto.ElectivePositionId
                   && cp.PoliticalPartyId == dto.PoliticalPartyId);

            if (positionAlreadyTaken)
                throw new InvalidOperationException(
                    "El partido ya tiene un candidato para este puesto.");

            var entity = _mapper.Map<CandidatePosition>(dto);
            entity.IsActive = true;

            await _candidatePositionRepository.AddAsync(entity);
        }
    }
}
