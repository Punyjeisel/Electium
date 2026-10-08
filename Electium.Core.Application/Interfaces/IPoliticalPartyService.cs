using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IPoliticalPartyService : IGenericService<PoliticalPartyDto, PoliticalParty>
    {
        Task<bool> IsAcronymTakenAsync(string acronym, int? excludePartyId = null);
    }
}
