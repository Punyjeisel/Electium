using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class ElectivePositionProfile : Profile
    {
        public ElectivePositionProfile()
        {
            CreateMap<ElectivePosition, ElectivePositionDto>().ReverseMap();

            CreateMap<CreateElectivePositionDto, ElectivePosition>();

            CreateMap<ElectivePosition, UpdateElectivePositionDto>();
            CreateMap<UpdateElectivePositionDto, ElectivePosition>();
        }
    }
}
