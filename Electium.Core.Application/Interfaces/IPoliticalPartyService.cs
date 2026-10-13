using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IPoliticalPartyService
    : IGenericService<PoliticalPartyDto, PoliticalParty>
    {
        Task<bool> IsAcronymTakenAsync(string acronym, int? excludePartyId = null);
        Task<bool> CanModifyAsync();
        Task<UpdatePoliticalPartyDto?> GetForEditAsync(int id);
        Task<OperationResult> CreateAsync(CreatePoliticalPartyDto dto);
        Task<OperationResult> UpdateAsync(UpdatePoliticalPartyDto dto);
        Task<OperationResult> ToggleActiveAsync(int id);
    }
}
