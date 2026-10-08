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

            // Password is hadled separately in UserService
            CreateMap<CreateUserDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());
        }
    }
}
