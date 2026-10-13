using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Mappings
{
    public class VotingProfile : Profile
    {
        public VotingProfile()
        {
            // Voting DTOs are built manually in the service
            // No mapping profile needed for now
        }
    }
}
