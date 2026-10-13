using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class PartyLeaderProfile : Profile
    {
        public PartyLeaderProfile()
        {
            CreateMap<PartyLeader, PartyLeaderDto>()
                .ForMember(d => d.UserFullName,
                    opt => opt.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(d => d.PoliticalPartyAcronym,
                    opt => opt.MapFrom(s => s.PoliticalParty.Acronym));

            CreateMap<CreatePartyLeaderDto, PartyLeader>();
        }
    }
}
