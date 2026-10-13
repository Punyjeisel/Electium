using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Enums;
using Electium.Core.Domain.Interfaces;

namespace Electium.Core.Application.Services;

public class AllianceService : IAllianceService
{
    private readonly IGenericRepository<AllianceRequest> _requestRepository;
    private readonly IGenericRepository<PoliticalAlliance> _allianceRepository;
    private readonly IMapper _mapper;

    public AllianceService(
        IGenericRepository<AllianceRequest> requestRepository,
        IGenericRepository<PoliticalAlliance> allianceRepository,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _allianceRepository = allianceRepository;
        _mapper = mapper;
    }

    // Requests sent to this party that are still pending
    public async Task<List<AllianceRequestDto>> GetReceivedPendingAsync(int partyId)
    {
        var list = await _requestRepository.GetAllAsync(
            r => r.ReceiverPartyId == partyId && r.Status == AllianceStatus.Pending,
            "RequesterParty",
            "ReceiverParty");

        return _mapper.Map<List<AllianceRequestDto>>(list);
    }

    // Requests sent by this party
    public async Task<List<AllianceRequestDto>> GetSentAsync(int partyId)
    {
        var list = await _requestRepository.GetAllAsync(
            r => r.RequesterPartyId == partyId,
            "RequesterParty",
            "ReceiverParty");

        return _mapper.Map<List<AllianceRequestDto>>(list);
    }

    // Active alliances of this party
    public async Task<List<PoliticalAllianceDto>> GetActiveAsync(int partyId)
    {
        var list = await _allianceRepository.GetAllAsync(
            a => a.PartyAId == partyId || a.PartyBId == partyId,
            "PartyA",
            "PartyB");

        // Map and swap so the "Ally" always shows the OTHER party
        return list.Select(a =>
        {
            var dto = _mapper.Map<PoliticalAllianceDto>(a);
            if (a.PartyAId == partyId)
            {
                dto.AllyPartyName = a.PartyB.Name;
                dto.AllyPartyAcronym = a.PartyB.Acronym;
            }
            else
            {
                dto.AllyPartyName = a.PartyA.Name;
                dto.AllyPartyAcronym = a.PartyA.Acronym;
            }
            return dto;
        }).ToList();
    }

    public async Task CreateRequestAsync(CreateAllianceRequestDto dto)
    {
        // Cannot send a request to yourself
        if (dto.RequesterPartyId == dto.ReceiverPartyId)
            throw new InvalidOperationException(
                "No puede enviar una solicitud a su propio partido.");

        // Rule: no pending request between both parties in either direction
        var hasPending = await _requestRepository.AnyAsync(r =>
            r.Status == AllianceStatus.Pending &&
            ((r.RequesterPartyId == dto.RequesterPartyId &&
              r.ReceiverPartyId == dto.ReceiverPartyId) ||
             (r.RequesterPartyId == dto.ReceiverPartyId &&
              r.ReceiverPartyId == dto.RequesterPartyId)));

        if (hasPending)
            throw new InvalidOperationException(
                "Ya existe una solicitud pendiente entre estos partidos.");

        var request = _mapper.Map<AllianceRequest>(dto);
        request.RequestDate = DateTime.UtcNow;
        request.Status = AllianceStatus.Pending;
        request.IsActive = true;

        await _requestRepository.AddAsync(request);
    }

    public async Task AcceptRequestAsync(int requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null || request.Status != AllianceStatus.Pending) return;

        request.Status = AllianceStatus.Accepted;
        await _requestRepository.UpdateAsync(request);

        // Create the formal alliance
        var alliance = new PoliticalAlliance
        {
            PartyAId = request.RequesterPartyId,
            PartyBId = request.ReceiverPartyId,
            AcceptedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _allianceRepository.AddAsync(alliance);
    }

    public async Task RejectRequestAsync(int requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null || request.Status != AllianceStatus.Pending) return;

        request.Status = AllianceStatus.Rejected;
        await _requestRepository.UpdateAsync(request);
    }

    public async Task DeleteRequestAsync(int requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null) return;

        request.IsActive = false;
        await _requestRepository.UpdateAsync(request);
    }
}