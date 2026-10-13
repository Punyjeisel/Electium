using AutoMapper;
using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Enums;
using Electium.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class CitizenService
    : GenericService<CitizenDto, Citizen>, ICitizenService
    {
        private readonly IGenericRepository<Citizen> _citizenRepository;
        private readonly IGenericRepository<Election> _electionRepository;

        public CitizenService(
            IGenericRepository<Citizen> citizenRepository,
            IGenericRepository<Election> electionRepository,
            IMapper mapper)
            : base(citizenRepository, mapper)
        {
            _citizenRepository = citizenRepository;
            _electionRepository = electionRepository;
        }

        public async Task<bool> IsIdentityNumberTakenAsync(
            string identityNumber, int? excludeCitizenId = null)
        {
            return await _citizenRepository.AnyAsync(c =>
                c.IdentityNumber == identityNumber &&
                (excludeCitizenId == null || c.Id != excludeCitizenId));
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        public async Task<UpdateCitizenDto?> GetForEditAsync(int id)
        {
            var entity = await _citizenRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<UpdateCitizenDto>(entity);
        }

        public async Task<OperationResult> CreateAsync(CreateCitizenDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede crear ciudadanos mientras exista una elección activa.");

            if (await IsIdentityNumberTakenAsync(dto.IdentityNumber))
                return OperationResult.Fail(
                    "Ya existe un ciudadano con ese número de documento.");

            var entity = _mapper.Map<Citizen>(dto);
            entity.IsActive = true;

            await _citizenRepository.AddAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateAsync(UpdateCitizenDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede editar ciudadanos mientras exista una elección activa.");

            if (await IsIdentityNumberTakenAsync(dto.IdentityNumber, dto.Id))
                return OperationResult.Fail(
                    "Ya existe otro ciudadano con ese número de documento.");

            var entity = await _citizenRepository.GetByIdAsync(dto.Id);
            if (entity == null)
                return OperationResult.Fail("El ciudadano no existe.");

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.IdentityNumber = dto.IdentityNumber;

            await _citizenRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> ToggleActiveAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede cambiar el estado de un ciudadano mientras exista una elección activa.");

            var entity = await _citizenRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("El ciudadano no existe.");

            entity.IsActive = !entity.IsActive;
            await _citizenRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }
    }
}
