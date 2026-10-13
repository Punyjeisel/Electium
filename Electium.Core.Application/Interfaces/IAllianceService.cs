using Electium.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IAllianceService
    {
        Task<List<AllianceRequestDto>> GetReceivedPendingAsync(int partyId);
        Task<List<AllianceRequestDto>> GetSentAsync(int partyId);
        Task<List<PoliticalAllianceDto>> GetActiveAsync(int partyId);
        Task CreateRequestAsync(CreateAllianceRequestDto dto);
        Task AcceptRequestAsync(int requestId);
        Task RejectRequestAsync(int requestId);
        Task DeleteRequestAsync(int requestId);
        Task<List<int>> GetAlliedPartyIdsAsync(int partyId);
        Task<bool> ArePartiesAlliedAsync(int partyAId, int partyBId);
    }


}
