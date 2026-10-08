using System;
using System.Collections.Generic;
using System.Text;
using Electium.Core.Domain.Common;
using Electium.Core.Application.Interfaces;
using AutoMapper;
using Electium.Core.Domain.Interface;

// Generic service implementation for CRUD operations on DTOs and entities

namespace Electium.Core.Application.Services
{
    public class GenericService<TDto, TEntity> : IGenericService<TDto, TEntity>
        where TDto : class
        where TEntity : BaseEntity
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TDto>(entity);
        }

        public virtual async Task<List<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<TDto>>(entities);
        }

        public virtual async Task AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.UpdateAsync(entity);
        }
    }
}
