using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface ICandidatePositionService
    : IGenericService<CandidatePositionDto, CandidatePosition>
    {
        Task<bool> CanModifyAsync();
        Task<List<CandidatePositionDto>> GetByPartyAsync(int partyId);
        Task<OperationResult> CreateAsync(CreateCandidatePositionDto dto);
        Task<OperationResult> DeleteAsync(int id);

        // Data for the create form
        Task<List<AvailableCandidateDto>> GetAvailableCandidatesAsync(int partyId);
        Task<List<AvailablePartyDto>> GetAvailablePositionsAsync(int partyId);

        // Check candidate restrictions
        Task<OperationResult> ValidateCandidateForPositionAsync(
            int partyId, int candidateId, int positionId);
    }
}
