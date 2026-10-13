using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Interfaces;
using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class LeaderDashboardService : ILeaderDashboardService
    {
        private readonly IGenericRepository<PoliticalParty> _partyRepository;
        private readonly IGenericRepository<Candidate> _candidateRepository;
        private readonly IGenericRepository<PoliticalAlliance> _allianceRepository;
        private readonly IGenericRepository<AllianceRequest> _requestRepository;
        private readonly IGenericRepository<CandidatePosition> _candidatePositionRepository;

        public LeaderDashboardService(
            IGenericRepository<PoliticalParty> partyRepository,
            IGenericRepository<Candidate> candidateRepository,
            IGenericRepository<PoliticalAlliance> allianceRepository,
            IGenericRepository<AllianceRequest> requestRepository,
            IGenericRepository<CandidatePosition> candidatePositionRepository)
        {
            _partyRepository = partyRepository;
            _candidateRepository = candidateRepository;
            _allianceRepository = allianceRepository;
            _requestRepository = requestRepository;
            _candidatePositionRepository = candidatePositionRepository;
        }

        public async Task<LeaderDashboardDto?> GetDashboardAsync(int partyId)
        {
            var party = await _partyRepository.GetByIdAsync(partyId);
            if (party == null) return null;

            var candidates = await _candidateRepository.GetAllAsync(
                c => c.PoliticalPartyId == partyId);

            var alliances = await _allianceRepository.GetAllAsync(
                a => a.IsActive && (a.PartyAId == partyId || a.PartyBId == partyId));

            var pending = await _requestRepository.GetAllAsync(
                r => r.ReceiverPartyId == partyId && r.Status == AllianceStatus.Pending);

            var assigned = await _candidatePositionRepository.GetAllAsync(
                cp => cp.PoliticalPartyId == partyId && cp.IsActive);

            return new LeaderDashboardDto
            {
                PartyName = party.Name,
                PartyAcronym = party.Acronym,
                PartyLogoPath = party.LogoPath,
                ActiveCandidates = candidates.Count(c => c.IsActive),
                InactiveCandidates = candidates.Count(c => !c.IsActive),
                ActiveAlliances = alliances.Count,
                PendingReceivedRequests = pending.Count,
                AssignedCandidates = assigned.Count
            };
        }
    }
}
