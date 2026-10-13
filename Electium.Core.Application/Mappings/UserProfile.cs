using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<CreateUserDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());
        }
    }
}
