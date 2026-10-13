using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Enums;
using Electium.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class CandidateService : GenericService<CandidateDto, Candidate>, ICandidateService
    {
        private readonly IGenericRepository<Candidate> _candidateRepository;
        private readonly IGenericRepository<Election> _electionRepository;

        public CandidateService(
            IGenericRepository<Candidate> candidateRepository,
            IGenericRepository<Election> electionRepository,
            IMapper mapper)
            : base(candidateRepository, mapper)
        {
            _candidateRepository = candidateRepository;
            _electionRepository = electionRepository;
        }

        // Check if there is an election in process
        public async Task<bool> HasActiveElectionAsync()
        {
            return await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);
        }

        public async Task<List<CandidateDto>> GetByPartyAsync(int partyId)
        {
            var candidates = await _candidateRepository.GetAllAsync(
                c => c.PoliticalPartyId == partyId,
                "PoliticalParty");

            return _mapper.Map<List<CandidateDto>>(candidates);
        }

        public async Task CreateAsync(CreateCandidateDto dto)
        {
            if (await HasActiveElectionAsync())
                throw new InvalidOperationException(
                    "No se puede crear candidatos durante una elección activa.");

            var candidate = _mapper.Map<Candidate>(dto);
            candidate.IsActive = true;

            await _candidateRepository.AddAsync(candidate);
        }

        public async Task UpdateAsync(UpdateCandidateDto dto)
        {
            if (await HasActiveElectionAsync())
                throw new InvalidOperationException(
                    "No se puede editar candidatos durante una elección activa.");

            var candidate = await _candidateRepository.GetByIdAsync(dto.Id);
            if (candidate == null) return;

            candidate.FirstName = dto.FirstName;
            candidate.LastName = dto.LastName;

            // Photo is optional during edit
            if (!string.IsNullOrWhiteSpace(dto.PhotoPath))
                candidate.PhotoPath = dto.PhotoPath;

            await _candidateRepository.UpdateAsync(candidate);
        }
    }
}
