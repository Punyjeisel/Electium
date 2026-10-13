using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class ElectionProfile : Profile
    {
        public ElectionProfile()
        {
            CreateMap<Election, ElectionDto>()
                .ForMember(d => d.PartyCount, opt => opt.Ignore())
                .ForMember(d => d.PositionCount, opt => opt.Ignore())
                .ForMember(d => d.TotalVotes, opt => opt.Ignore());

            CreateMap<CreateElectionDto, Election>();
        }
    }
}
