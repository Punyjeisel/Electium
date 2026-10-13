using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface ICandidatePositionService : IGenericService<CandidatePositionDto, CandidatePosition>
    {
        Task<List<CandidatePositionDto>> GetByPartyAsync(int partyId);
        Task CreateAsync(CreateCandidatePositionDto dto);
    }
}
