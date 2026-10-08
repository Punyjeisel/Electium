using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

// Generi

namespace Electium.Core.Application.Interfaces
{
    public interface IGenericService<TDto, TEntity> 
        where TDto : class 
        where TEntity : BaseEntity
    {
        Task<TDto?> GetByIdAsync(int id);
        Task<List<TDto>> GetAllAsync();
        Task AddAsync(TDto dto);
        Task UpdateAsync(TDto dto);
    }
}
