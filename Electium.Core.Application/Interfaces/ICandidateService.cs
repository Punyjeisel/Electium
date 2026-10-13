using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface ICandidateService : IGenericService<CandidateDto, Candidate>
    {
        Task<List<CandidateDto>> GetByPartyAsync(int partyId);
        Task<bool> HasActiveElectionAsync();
        Task CreateAsync(CreateCandidateDto dto);
        Task UpdateAsync(UpdateCandidateDto dto);
    }
}
