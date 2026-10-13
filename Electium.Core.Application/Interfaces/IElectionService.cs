using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IElectionService : IGenericService<ElectionDto, Election>
    {
        Task<bool> HasActiveElectionAsync();
        Task<List<ElectionDto>> GetAllOrderedAsync();
        Task<CreateElectionDto?> GetCreateFormDataAsync();
        Task<OperationResult> CreateAsync(CreateElectionDto dto);
        Task<FinishElectionViewModelDto?> GetFinishInfoAsync(int id);
        Task<OperationResult> FinishAsync(int electionId);
        Task<List<ElectionResultDto>?> GetResultsAsync(int electionId);
    }
}
