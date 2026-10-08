using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

// Generic contract for repository pattern, to be implemented by all repositories in the project

namespace Electium.Core.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            params string[] includes);

        Task<bool> AnyAsync(Expression<Func<T, bool>>? filter);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);
    }
}
