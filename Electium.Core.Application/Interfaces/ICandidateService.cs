using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface ICandidateService : IGenericService<CandidateDto, Candidate>
    {
        Task<bool> CanModifyAsync();
        Task<List<CandidateDto>> GetByPartyAsync(int partyId);
        Task<UpdateCandidateDto?> GetForEditAsync(int id);
        Task<OperationResult> CreateAsync(CreateCandidateDto dto);
        Task<OperationResult> UpdateAsync(UpdateCandidateDto dto);
        Task<OperationResult> ToggleActiveAsync(int id);
    }
}
