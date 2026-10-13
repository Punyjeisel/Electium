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
    public class ElectionService
    : GenericService<ElectionDto, Election>, IElectionService
    {
        private readonly IGenericRepository<Election> _electionRepository;
        private readonly IGenericRepository<ElectivePosition> _positionRepository;
        private readonly IGenericRepository<PoliticalParty> _partyRepository;
        private readonly IGenericRepository<CandidatePosition> _candidatePositionRepository;
        private readonly IGenericRepository<ElectionCandidate> _electionCandidateRepository;
        private readonly IGenericRepository<ElectionVoter> _electionVoterRepository;
        private readonly IGenericRepository<Vote> _voteRepository;

        public ElectionService(
            IGenericRepository<Election> electionRepository,
            IGenericRepository<ElectivePosition> positionRepository,
            IGenericRepository<PoliticalParty> partyRepository,
            IGenericRepository<CandidatePosition> candidatePositionRepository,
            IGenericRepository<ElectionCandidate> electionCandidateRepository,
            IGenericRepository<ElectionVoter> electionVoterRepository,
            IGenericRepository<Vote> voteRepository,
            IMapper mapper)
            : base(electionRepository, mapper)
        {
            _electionRepository = electionRepository;
            _positionRepository = positionRepository;
            _partyRepository = partyRepository;
            _candidatePositionRepository = candidatePositionRepository;
            _electionCandidateRepository = electionCandidateRepository;
            _electionVoterRepository = electionVoterRepository;
            _voteRepository = voteRepository;
        }

        public async Task<bool> HasActiveElectionAsync()
        {
            return await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);
        }

        public async Task<List<ElectionDto>> GetAllOrderedAsync()
        {
            var elections = await _electionRepository.GetAllAsync();

            // Active first, then by date desc
            return elections
                .OrderByDescending(e => e.Status == ElectionStatus.InProcess)
                .ThenByDescending(e => e.Date)
                .Select(e => _mapper.Map<ElectionDto>(e))
                .ToList();
        }

        public async Task CreateAsync(CreateElectionDto dto)
        {
            if (await HasActiveElectionAsync())
                throw new InvalidOperationException(
                    "Ya existe una elección activa en el sistema.");

            var activePositions = await _positionRepository.GetAllAsync(p => p.IsActive);
            if (!activePositions.Any())
                throw new InvalidOperationException(
                    "No hay puestos electivos activos para realizar una elección.");

            var activeParties = await _partyRepository.GetAllAsync(p => p.IsActive);
            if (activeParties.Count < 2)
                throw new InvalidOperationException(
                    "No hay suficientes partidos políticos para realizar una elección.");

            // For each party, check it has a candidate for every active position
            var missingMessages = new List<string>();
            foreach (var party in activeParties)
            {
                var positionsWithoutCandidate = new List<string>();

                foreach (var position in activePositions)
                {
                    var hasCandidate = await _candidatePositionRepository.AnyAsync(
                        cp => cp.PoliticalPartyId == party.Id
                           && cp.ElectivePositionId == position.Id
                           && cp.IsActive);

                    if (!hasCandidate)
                        positionsWithoutCandidate.Add(position.Name);
                }

                if (positionsWithoutCandidate.Any())
                {
                    missingMessages.Add(
                        $"El partido político {party.Name} {party.Acronym} " +
                        $"no tiene candidatos registrados para los siguientes " +
                        $"puestos electivos: {string.Join(", ", positionsWithoutCandidate)}.");
                }
            }

            if (missingMessages.Any())
                throw new InvalidOperationException(string.Join(" | ", missingMessages));

            // Create the election
            var election = _mapper.Map<Election>(dto);
            election.Status = ElectionStatus.InProcess;
            election.IsActive = true;

            await _electionRepository.AddAsync(election);

            // Take the snapshot of participants
            foreach (var party in activeParties)
            {
                var assignments = await _candidatePositionRepository.GetAllAsync(
                    cp => cp.PoliticalPartyId == party.Id && cp.IsActive);

                foreach (var assignment in assignments)
                {
                    var ec = new ElectionCandidate
                    {
                        ElectionId = election.Id,
                        ElectivePositionId = assignment.ElectivePositionId,
                        CandidateId = assignment.CandidateId,
                        PoliticalPartyId = party.Id,
                        IsActive = true
                    };
                    await _electionCandidateRepository.AddAsync(ec);
                }
            }
        }

        public async Task FinishAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null) return;

            election.Status = ElectionStatus.Finished;
            await _electionRepository.UpdateAsync(election);
        }

        public async Task<List<ElectionResultDto>> GetResultsAsync(int electionId)
        {
            var participants = await _electionCandidateRepository.GetAllAsync(
                ec => ec.ElectionId == electionId,
                "ElectivePosition",
                "Candidate",
                "PoliticalParty");

            var votes = await _voteRepository.GetAllAsync(v => v.ElectionId == electionId);

            // Group by position
            var results = participants
                .GroupBy(p => new { p.ElectivePositionId, p.ElectivePosition.Name })
                .Select(g =>
                {
                    var positionVotes = votes.Where(v => v.ElectivePositionId == g.Key.ElectivePositionId).ToList();
                    var totalForPosition = positionVotes.Count;

                    var candidateResults = g
                        .GroupBy(p => new { p.CandidateId, p.Candidate, p.PoliticalParty })
                        .Select(cg =>
                        {
                            var candidateVotes = positionVotes.Count(v => v.CandidateId == cg.Key.CandidateId);
                            var percentage = totalForPosition == 0
                                ? 0
                                : (double)candidateVotes / totalForPosition * 100;

                            return new CandidateResultDto
                            {
                                CandidateName = cg.Key.Candidate.FirstName + " " + cg.Key.Candidate.LastName,
                                PartyAcronym = cg.Key.PoliticalParty.Acronym,
                                Votes = candidateVotes,
                                Percentage = Math.Round(percentage, 2)
                            };
                        })
                        .OrderByDescending(c => c.Votes)
                        .ToList();

                    return new ElectionResultDto
                    {
                        PositionName = g.Key.Name,
                        TotalVotesForPosition = totalForPosition,
                        Candidates = candidateResults
                    };
                })
                .ToList();

            return results;
        }
    }
}
