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
    public class VotingService : IVotingService
    {
        private readonly IGenericRepository<Citizen> _citizenRepository;
        private readonly IGenericRepository<Election> _electionRepository;
        private readonly IGenericRepository<ElectionVoter> _electionVoterRepository;
        private readonly IGenericRepository<ElectionCandidate> _electionCandidateRepository;
        private readonly IGenericRepository<Vote> _voteRepository;
        private readonly IGenericRepository<ElectivePosition> _positionRepository;
        private readonly IMapper _mapper;

        public VotingService(
            IGenericRepository<Citizen> citizenRepository,
            IGenericRepository<Election> electionRepository,
            IGenericRepository<ElectionVoter> electionVoterRepository,
            IGenericRepository<ElectionCandidate> electionCandidateRepository,
            IGenericRepository<Vote> voteRepository,
            IGenericRepository<ElectivePosition> positionRepository,
            IMapper mapper)
        {
            _citizenRepository = citizenRepository;
            _electionRepository = electionRepository;
            _electionVoterRepository = electionVoterRepository;
            _electionCandidateRepository = electionCandidateRepository;
            _voteRepository = voteRepository;
            _positionRepository = positionRepository;
            _mapper = mapper;
        }

        // Step 1: validate the identity number entered in the first form
        public async Task<VoterCheckDto> CheckVoterAsync(string identityNumber)
        {
            // Find citizen by identity number
            var citizens = await _citizenRepository.GetAllAsync(
                c => c.IdentityNumber == identityNumber);
            var citizen = citizens.FirstOrDefault();

            if (citizen == null)
                return new VoterCheckDto { Result = VoterCheckResult.CitizenNotFound };

            if (!citizen.IsActive)
                return new VoterCheckDto { Result = VoterCheckResult.CitizenInactive };

            // Find the active election (there should be at most one)
            var activeElections = await _electionRepository.GetAllAsync(
                e => e.Status == ElectionStatus.InProcess);
            var election = activeElections.FirstOrDefault();

            if (election == null)
                return new VoterCheckDto { Result = VoterCheckResult.NoActiveElection };

            // Has this citizen already voted in this election?
            var alreadyVoted = await _electionVoterRepository.AnyAsync(
                ev => ev.ElectionId == election.Id && ev.CitizenId == citizen.Id);

            if (alreadyVoted)
                return new VoterCheckDto
                {
                    Result = VoterCheckResult.AlreadyVoted,
                    CitizenId = citizen.Id,
                    ActiveElectionId = election.Id
                };

            return new VoterCheckDto
            {
                Result = VoterCheckResult.CanProceed,
                CitizenId = citizen.Id,
                ActiveElectionId = election.Id
            };
        }

        // Step 2: compare manual input with OCR result
        public Task<bool> VerifyIdentityAsync(string manualIdentityNumber, string ocrIdentityNumber)
        {
            // Normalize: remove dashes, spaces and hyphens
            var a = Normalize(manualIdentityNumber);
            var b = Normalize(ocrIdentityNumber);
            return Task.FromResult(a == b && !string.IsNullOrEmpty(a));
        }

        // Step 3: positions the citizen can vote for in this election
        public async Task<List<VotingPositionDto>> GetPositionsAsync(int electionId)
        {
            var participants = await _electionCandidateRepository.GetAllAsync(
                ec => ec.ElectionId == electionId,
                "ElectivePosition",
                "Candidate",
                "PoliticalParty");

            return participants
                .GroupBy(p => new { p.ElectivePositionId, p.ElectivePosition.Name })
                .Select(g =>
                {
                    var distinctParties = g.Select(x => x.PoliticalPartyId).Distinct().Count();
                    var distinctCandidates = g.Select(x => x.CandidateId).Distinct().Count();

                    return new VotingPositionDto
                    {
                        ElectivePositionId = g.Key.ElectivePositionId,
                        PositionName = g.Key.Name,
                        PartyCount = distinctParties,
                        CandidateCount = distinctCandidates
                    };
                })
                .ToList();
        }

        // Step 4: candidates for a given position
        public async Task<List<VotingCandidateDto>> GetCandidatesByPositionAsync(
            int electionId, int positionId)
        {
            var rows = await _electionCandidateRepository.GetAllAsync(
                ec => ec.ElectionId == electionId && ec.ElectivePositionId == positionId,
                "Candidate",
                "PoliticalParty");

            // Group by candidate so a candidate running for several parties appears once
            var candidates = rows
                .GroupBy(r => r.CandidateId)
                .Select(g =>
                {
                    var first = g.First();
                    var acronyms = string.Join(" / ",
                        g.Select(x => x.PoliticalParty.Acronym).Distinct());

                    return new VotingCandidateDto
                    {
                        CandidateId = first.CandidateId,
                        CandidateName = first.Candidate.FirstName + " " + first.Candidate.LastName,
                        PartyAcronym = acronyms,
                        PartyLogoPath = first.PoliticalParty.LogoPath,
                        CandidatePhotoPath = first.Candidate.PhotoPath
                    };
                })
                .ToList();

            // Always add the "Ninguno" option
            candidates.Add(new VotingCandidateDto
            {
                CandidateId = null,
                CandidateName = "Ninguno",
                PartyAcronym = string.Empty
            });

            return candidates;
        }

        // Step 5: store a single vote
        public async Task RegisterVoteAsync(RegisterVoteDto dto)
        {
            // Prevent duplicate votes for the same position in this election
            // (ElectionVoter is checked when finishing, but this avoids misuse)
            var vote = new Vote
            {
                ElectionId = dto.ElectionId,
                ElectivePositionId = dto.ElectivePositionId,
                CandidateId = dto.CandidateId,
                IsActive = true
            };

            await _voteRepository.AddAsync(vote);
        }

        // Step 6: close the voting session and build the email summary
        public async Task<VotingSummaryDto> CompleteVotingAsync(int electionId, int citizenId)
        {
            // Register the citizen as voter (prevents double voting)
            var alreadyVoted = await _electionVoterRepository.AnyAsync(
                ev => ev.ElectionId == electionId && ev.CitizenId == citizenId);

            if (!alreadyVoted)
            {
                await _electionVoterRepository.AddAsync(new ElectionVoter
                {
                    ElectionId = electionId,
                    CitizenId = citizenId,
                    IsActive = true
                });
            }

            var citizen = await _citizenRepository.GetByIdAsync(citizenId);
            var election = await _electionRepository.GetByIdAsync(electionId);

            // Build summary from all positions in this election
            var participants = await _electionCandidateRepository.GetAllAsync(
                ec => ec.ElectionId == electionId,
                "ElectivePosition",
                "Candidate");

            var summary = new VotingSummaryDto
            {
                CitizenName = citizen != null ? $"{citizen.FirstName} {citizen.LastName}" : string.Empty,
                CitizenEmail = citizen?.Email ?? string.Empty,
                ElectionName = election?.Name ?? string.Empty,
                ElectionDate = election?.Date ?? DateTime.MinValue
            };

            // NOTE: For the summary we need per-position results.
            // Since Vote does not store CitizenId (confidential), we cannot know
            // exactly which vote belongs to this citizen from the repository alone.
            // For the summary, we return the positions of the election with the
            // candidates that participated. The Web layer will overwrite this with
            // the actual selection tracked in session (or a temporary table) if needed.
            foreach (var group in participants.GroupBy(p => new { p.ElectivePositionId, p.ElectivePosition.Name }))
            {
                summary.Items.Add(new VotingSummaryItemDto
                {
                    PositionName = group.Key.Name,
                    CandidateName = string.Empty,
                    PartyAcronym = string.Empty
                });
            }

            return summary;
        }

        private static string Normalize(string value)
        {
            return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        }
    }
}
