using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;

namespace Electium.Core.Application.Mappings;

public class AllianceProfile : Profile
{
    public AllianceProfile()
    {
        CreateMap<AllianceRequest, AllianceRequestDto>()
            .ForMember(d => d.RequesterPartyName,
                opt => opt.MapFrom(s => s.RequesterParty.Name))
            .ForMember(d => d.RequesterPartyAcronym,
                opt => opt.MapFrom(s => s.RequesterParty.Acronym))
            .ForMember(d => d.ReceiverPartyName,
                opt => opt.MapFrom(s => s.ReceiverParty.Name))
            .ForMember(d => d.ReceiverPartyAcronym,
                opt => opt.MapFrom(s => s.ReceiverParty.Acronym));

        CreateMap<CreateAllianceRequestDto, AllianceRequest>();

        CreateMap<PoliticalAlliance, PoliticalAllianceDto>()
            .ForMember(d => d.AllyPartyName, opt => opt.Ignore())
            .ForMember(d => d.AllyPartyAcronym, opt => opt.Ignore());
    }
}