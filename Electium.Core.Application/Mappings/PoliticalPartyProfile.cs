using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class PoliticalPartyProfile : Profile
    {
        public PoliticalPartyProfile()
        {
            CreateMap<PoliticalParty, PoliticalPartyDto>().ReverseMap();

            CreateMap<CreatePoliticalPartyDto, PoliticalParty>();

            CreateMap<PoliticalParty, UpdatePoliticalPartyDto>();
            CreateMap<UpdatePoliticalPartyDto, PoliticalParty>()
                .ForMember(d => d.LogoPath, opt => opt.Ignore());
        }
    }
}
