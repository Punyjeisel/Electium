using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface ICitizenService : IGenericService<CitizenDto, Citizen>
    {
        Task<bool> IsIdentityNumberTakenAsync(string identityNumber, int? excludeCitizenId = null);
    }
}
