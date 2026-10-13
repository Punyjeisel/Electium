using AutoMapper;
using Electium.Core.Application.Common;
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
    public class CandidateService
    : GenericService<CandidateDto, Candidate>, ICandidateService
    {
        private readonly IGenericRepository<Candidate> _candidateRepository;
        private readonly IGenericRepository<CandidatePosition> _candidatePositionRepository;
        private readonly IGenericRepository<ElectivePosition> _positionRepository;
        private readonly IGenericRepository<Election> _electionRepository;

        public CandidateService(
            IGenericRepository<Candidate> candidateRepository,
            IGenericRepository<CandidatePosition> candidatePositionRepository,
            IGenericRepository<ElectivePosition> positionRepository,
            IGenericRepository<Election> electionRepository,
            IMapper mapper)
            : base(candidateRepository, mapper)
        {
            _candidateRepository = candidateRepository;
            _candidatePositionRepository = candidatePositionRepository;
            _positionRepository = positionRepository;
            _electionRepository = electionRepository;
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        // Returns candidates of a party with their assigned position (if any)
        public async Task<List<CandidateDto>> GetByPartyAsync(int partyId)
        {
            var candidates = await _candidateRepository.GetAllAsync(
                c => c.PoliticalPartyId == partyId,
                "PoliticalParty");

            var result = new List<CandidateDto>();
            foreach (var candidate in candidates)
            {
                var dto = _mapper.Map<CandidateDto>(candidate);

                // Look for an active assignment for this candidate in this party
                var assignments = await _candidatePositionRepository.GetAllAsync(
                    cp => cp.CandidateId == candidate.Id
                       && cp.PoliticalPartyId == partyId
                       && cp.IsActive,
                    "ElectivePosition");

                var assignment = assignments.FirstOrDefault();
                dto.AssignedPositionName = assignment?.ElectivePosition?.Name;

                result.Add(dto);
            }

            return result;
        }

        public async Task<UpdateCandidateDto?> GetForEditAsync(int id)
        {
            var entity = await _candidateRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<UpdateCandidateDto>(entity);
        }

        public async Task<OperationResult> CreateAsync(CreateCandidateDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede crear candidatos mientras exista una elección activa.");

            var entity = _mapper.Map<Candidate>(dto);
            entity.IsActive = true;

            await _candidateRepository.AddAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateAsync(UpdateCandidateDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede editar candidatos mientras exista una elección activa.");

            var entity = await _candidateRepository.GetByIdAsync(dto.Id);
            if (entity == null)
                return OperationResult.Fail("El candidato no existe.");

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;

            // Photo is optional during edit
            if (!string.IsNullOrWhiteSpace(dto.PhotoPath))
                entity.PhotoPath = dto.PhotoPath;

            await _candidateRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> ToggleActiveAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede cambiar el estado de un candidato mientras exista una elección activa.");

            var entity = await _candidateRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("El candidato no existe.");

            entity.IsActive = !entity.IsActive;
            await _candidateRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }
    }

}
