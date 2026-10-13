using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IElectivePositionService
    : IGenericService<ElectivePositionDto, ElectivePosition>
    {
        Task<bool> CanModifyAsync();
        Task<UpdateElectivePositionDto?> GetForEditAsync(int id);
        Task<OperationResult> CreateAsync(CreateElectivePositionDto dto);
        Task<OperationResult> UpdateAsync(UpdateElectivePositionDto dto);
        Task<OperationResult> ToggleActiveAsync(int id);
    }
}
