using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IPartyLeaderService : IGenericService<PartyLeaderDto, PartyLeader>
    {
        Task<List<PartyLeaderDto>> GetAllWithIncludesAsync();
        Task<bool> IsUserAlreadyAssignedAsync(int userId);
        Task<bool> CanModifyAsync();
        Task<OperationResult> CreateAsync(CreatePartyLeaderDto dto);
        Task<OperationResult> DeleteAsync(int id);
        Task<List<AvailableLeaderDto>> GetAvailableLeadersAsync();
        Task<List<AvailablePartyDto>> GetActivePartiesAsync();
        Task<int?> GetPartyIdByUserIdAsync(int userId);
    }
}
