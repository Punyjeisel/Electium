using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class CandidateProfile : Profile
    {
        public CandidateProfile()
        {
            CreateMap<Candidate, CandidateDto>()
                .ForMember(d => d.PoliticalPartyAcronym,
                    opt => opt.MapFrom(s => s.PoliticalParty.Acronym));

            CreateMap<CreateCandidateDto, Candidate>();

            CreateMap<UpdateCandidateDto, Candidate>()
                .ForMember(d => d.PhotoPath, opt => opt.Ignore());
        }
    }
}
