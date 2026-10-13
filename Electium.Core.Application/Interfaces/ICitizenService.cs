using Electium.Core.Application.Common;
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

        Task<bool> CanModifyAsync();
        Task<UpdateCitizenDto?> GetForEditAsync(int id);
        Task<OperationResult> CreateAsync(CreateCitizenDto dto);
        Task<OperationResult> UpdateAsync(UpdateCitizenDto dto);
        Task<OperationResult> ToggleActiveAsync(int id);
    }
}
