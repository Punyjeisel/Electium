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
    public class CandidatePositionService
    : GenericService<CandidatePositionDto, CandidatePosition>, ICandidatePositionService
    {
        private readonly IGenericRepository<CandidatePosition> _candidatePositionRepository;
        private readonly IGenericRepository<Candidate> _candidateRepository;
        private readonly IGenericRepository<ElectivePosition> _positionRepository;
        private readonly IGenericRepository<Election> _electionRepository;
        private readonly IAllianceService _allianceService;

        public CandidatePositionService(
            IGenericRepository<CandidatePosition> candidatePositionRepository,
            IGenericRepository<Candidate> candidateRepository,
            IGenericRepository<ElectivePosition> positionRepository,
            IGenericRepository<Election> electionRepository,
            IAllianceService allianceService,
            IMapper mapper)
            : base(candidatePositionRepository, mapper)
        {
            _candidatePositionRepository = candidatePositionRepository;
            _candidateRepository = candidateRepository;
            _positionRepository = positionRepository;
            _electionRepository = electionRepository;
            _allianceService = allianceService;
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        public async Task<List<CandidatePositionDto>> GetByPartyAsync(int partyId)
        {
            var list = await _candidatePositionRepository.GetAllAsync(
                cp => cp.PoliticalPartyId == partyId && cp.IsActive,
                "Candidate",
                "ElectivePosition");

            return _mapper.Map<List<CandidatePositionDto>>(list);
        }

        // Candidates available to assign:
        // - Active candidates of this party without an active assignment in this party
        // - Active candidates of allied parties, but only for the same position they
        //   already have in their own party (checked in ValidateCandidateForPositionAsync)
        public async Task<List<AvailableCandidateDto>> GetAvailableCandidatesAsync(int partyId)
        {
            var result = new List<AvailableCandidateDto>();

            // Own candidates
            var ownCandidates = await _candidateRepository.GetAllAsync(
                c => c.PoliticalPartyId == partyId && c.IsActive,
                "PoliticalParty");

            foreach (var candidate in ownCandidates)
            {
                // If already assigned in this party, skip
                var alreadyAssigned = await _candidatePositionRepository.AnyAsync(
                    cp => cp.CandidateId == candidate.Id
                       && cp.PoliticalPartyId == partyId
                       && cp.IsActive);

                if (alreadyAssigned) continue;

                result.Add(new AvailableCandidateDto
                {
                    CandidateId = candidate.Id,
                    FullName = $"{candidate.FirstName} {candidate.LastName}",
                    PartyAcronym = candidate.PoliticalParty.Acronym,
                    IsFromAlly = false
                });
            }

            // Allied parties' candidates
            var alliedPartyIds = await _allianceService.GetAlliedPartyIdsAsync(partyId);
            foreach (var allyId in alliedPartyIds)
            {
                var allyCandidates = await _candidateRepository.GetAllAsync(
                    c => c.PoliticalPartyId == allyId && c.IsActive,
                    "PoliticalParty");

                foreach (var candidate in allyCandidates)
                {
                    result.Add(new AvailableCandidateDto
                    {
                        CandidateId = candidate.Id,
                        FullName = $"{candidate.FirstName} {candidate.LastName}",
                        PartyAcronym = candidate.PoliticalParty.Acronym,
                        IsFromAlly = true
                    });
                }
            }

            return result;
        }

        // Positions available to assign:
        // - Active positions that don't yet have a candidate inside this party
        public async Task<List<AvailablePartyDto>> GetAvailablePositionsAsync(int partyId)
        {
            var positions = await _positionRepository.GetAllAsync(p => p.IsActive);

            var result = new List<AvailablePartyDto>();
            foreach (var position in positions)
            {
                var taken = await _candidatePositionRepository.AnyAsync(
                    cp => cp.ElectivePositionId == position.Id
                       && cp.PoliticalPartyId == partyId
                       && cp.IsActive);

                if (taken) continue;

                result.Add(new AvailablePartyDto
                {
                    Id = position.Id,
                    Acronym = position.Name,
                    Name = position.Description
                });
            }

            return result;
        }

        public async Task<OperationResult> ValidateCandidateForPositionAsync(
            int partyId, int candidateId, int positionId)
        {
            var candidate = await _candidateRepository.GetByIdAsync(candidateId);
            if (candidate == null)
                return OperationResult.Fail("El candidato no existe.");

            // Own candidate?
            if (candidate.PoliticalPartyId == partyId)
            {
                var alreadyAssigned = await _candidatePositionRepository.AnyAsync(
                    cp => cp.CandidateId == candidateId
                       && cp.PoliticalPartyId == partyId
                       && cp.IsActive);

                if (alreadyAssigned)
                    return OperationResult.Fail(
                        "Este candidato ya está asignado a un puesto dentro del partido.");

                return OperationResult.Ok();
            }

            // Ally candidate - must be allied and aspire to the same position
            var allied = await _allianceService.ArePartiesAlliedAsync(
                partyId, candidate.PoliticalPartyId);

            if (!allied)
                return OperationResult.Fail(
                    "El candidato no pertenece a su partido ni a un partido aliado.");

            // Original party position of this candidate
            var originalAssignments = await _candidatePositionRepository.GetAllAsync(
                cp => cp.CandidateId == candidateId
                   && cp.PoliticalPartyId == candidate.PoliticalPartyId
                   && cp.IsActive);

            var originalPosition = originalAssignments.FirstOrDefault();

            if (originalPosition == null)
                return OperationResult.Fail(
                    "El candidato aliado aún no tiene un puesto asignado en su partido de origen.");

            if (originalPosition.ElectivePositionId != positionId)
                return OperationResult.Fail(
                    "Este candidato en su partido de origen aspira a un puesto diferente al seleccionado.");

            return OperationResult.Ok();
        }

        public async Task<OperationResult> CreateAsync(CreateCandidatePositionDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede asignar candidatos mientras exista una elección activa.");

            // Position already taken in this party?
            var positionTaken = await _candidatePositionRepository.AnyAsync(
                cp => cp.ElectivePositionId == dto.ElectivePositionId
                   && cp.PoliticalPartyId == dto.PoliticalPartyId
                   && cp.IsActive);

            if (positionTaken)
                return OperationResult.Fail(
                    "El partido ya tiene un candidato para este puesto.");

            // Validate candidate rules
            var validation = await ValidateCandidateForPositionAsync(
                dto.PoliticalPartyId, dto.CandidateId, dto.ElectivePositionId);

            if (!validation.Success) return validation;

            var entity = _mapper.Map<CandidatePosition>(dto);
            entity.IsActive = true;

            await _candidatePositionRepository.AddAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede desvincular candidatos mientras exista una elección activa.");

            var entity = await _candidatePositionRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("La asignación no existe.");

            entity.IsActive = false;
            await _candidatePositionRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }
    }

}
