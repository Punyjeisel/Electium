using AutoMapper;
using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Enums;
using Electium.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class PoliticalPartyService
    : GenericService<PoliticalPartyDto, PoliticalParty>, IPoliticalPartyService
    {
        private readonly IGenericRepository<PoliticalParty> _partyRepository;
        private readonly IGenericRepository<Candidate> _candidateRepository;
        private readonly IGenericRepository<Election> _electionRepository;

        public PoliticalPartyService(
            IGenericRepository<PoliticalParty> partyRepository,
            IGenericRepository<Candidate> candidateRepository,
            IGenericRepository<Election> electionRepository,
            IMapper mapper)
            : base(partyRepository, mapper)
        {
            _partyRepository = partyRepository;
            _candidateRepository = candidateRepository;
            _electionRepository = electionRepository;
        }

        public async Task<bool> IsAcronymTakenAsync(string acronym, int? excludePartyId = null)
        {
            return await _partyRepository.AnyAsync(p =>
                p.Acronym == acronym &&
                (excludePartyId == null || p.Id != excludePartyId));
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        public async Task<UpdatePoliticalPartyDto?> GetForEditAsync(int id)
        {
            var entity = await _partyRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<UpdatePoliticalPartyDto>(entity);
        }

        public async Task<OperationResult> CreateAsync(CreatePoliticalPartyDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede crear partidos mientras exista una elección activa.");

            if (await IsAcronymTakenAsync(dto.Acronym))
                return OperationResult.Fail(
                    "Ya existe un partido con esas siglas.");

            var entity = _mapper.Map<PoliticalParty>(dto);
            entity.IsActive = true;

            await _partyRepository.AddAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateAsync(UpdatePoliticalPartyDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede editar partidos mientras exista una elección activa.");

            if (await IsAcronymTakenAsync(dto.Acronym, dto.Id))
                return OperationResult.Fail(
                    "Ya existe otro partido con esas siglas.");

            var entity = await _partyRepository.GetByIdAsync(dto.Id);
            if (entity == null)
                return OperationResult.Fail("El partido no existe.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Acronym = dto.Acronym;

            // Logo is optional during edit
            if (!string.IsNullOrWhiteSpace(dto.LogoPath))
                entity.LogoPath = dto.LogoPath;

            await _partyRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> ToggleActiveAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede cambiar el estado de un partido mientras exista una elección activa.");

            var entity = await _partyRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("El partido no existe.");

            entity.IsActive = !entity.IsActive;

            // If the party is deactivated, all its candidates must also be deactivated
            if (!entity.IsActive)
            {
                var candidates = await _candidateRepository.GetAllAsync(
                    c => c.PoliticalPartyId == entity.Id && c.IsActive);

                foreach (var candidate in candidates)
                {
                    candidate.IsActive = false;
                    await _candidateRepository.UpdateAsync(candidate);
                }
            }

            await _partyRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }
    }
}
