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

            var result = new List<ElectionDto>();

            foreach (var election in elections)
            {
                var dto = _mapper.Map<ElectionDto>(election);

                // Participants snapshot
                var participants = await _electionCandidateRepository.GetAllAsync(
                    ec => ec.ElectionId == election.Id);

                dto.PartyCount = participants.Select(p => p.PoliticalPartyId).Distinct().Count();
                dto.PositionCount = participants.Select(p => p.ElectivePositionId).Distinct().Count();

                // Voters
                var voters = await _electionVoterRepository.GetAllAsync(
                    ev => ev.ElectionId == election.Id);
                dto.TotalVotes = voters.Count;

                result.Add(dto);
            }

            return result
                .OrderByDescending(e => e.Status == ElectionStatus.InProcess)
                .ThenByDescending(e => e.Date)
                .ToList();
        }

        public async Task<OperationResult> CreateAsync(CreateElectionDto dto)
        {
            if (await HasActiveElectionAsync())
                return OperationResult.Fail(
                    "Ya existe una elección activa en el sistema.");

            var activePositions = await _positionRepository.GetAllAsync(p => p.IsActive);
            if (!activePositions.Any())
                return OperationResult.Fail(
                    "No hay puestos electivos activos para realizar una elección.");

            var activeParties = await _partyRepository.GetAllAsync(p => p.IsActive);
            if (activeParties.Count < 2)
                return OperationResult.Fail(
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
                return OperationResult.Fail(string.Join(" | ", missingMessages));

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

            return OperationResult.Ok();
        }

        public async Task<OperationResult> FinishAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
                return OperationResult.Fail("La elección no existe.");

            election.Status = ElectionStatus.Finished;
            await _electionRepository.UpdateAsync(election);

            return OperationResult.Ok();
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

        public Task<CreateElectionDto?> GetCreateFormDataAsync()
        {
            // No form data to preload; placeholder for future validations
            return Task.FromResult<CreateElectionDto?>(new CreateElectionDto
            {
                Date = DateTime.Today
            });
        }

        public async Task<FinishElectionViewModelDto?> GetFinishInfoAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null) return null;

            return new FinishElectionViewModelDto
            {
                Id = election.Id,
                Name = election.Name
            };
        }
    }
}
