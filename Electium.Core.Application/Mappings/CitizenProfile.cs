using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Services;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class CitizenProfile : Profile
    {
        public CitizenProfile()
        {
            CreateMap<Citizen, CitizenDto>().ReverseMap();

            CreateMap<CreateCitizenDto, Citizen>();

            CreateMap<Citizen, UpdateCitizenDto>();
            CreateMap<UpdateCitizenDto, Citizen>();
        }
    }
}
