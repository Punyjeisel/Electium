using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class CandidatePositionProfile : Profile
    {
        public CandidatePositionProfile()
        {
            CreateMap<CandidatePosition, CandidatePositionDto>()
                .ForMember(d => d.CandidateFullName,
                    opt => opt.MapFrom(s =>
                        s.Candidate.FirstName + " " + s.Candidate.LastName))
                .ForMember(d => d.ElectivePositionName,
                    opt => opt.MapFrom(s => s.ElectivePosition.Name));

            CreateMap<CreateCandidatePositionDto, CandidatePosition>();
        }
    }
}
