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
    public class ElectivePositionService
    : GenericService<ElectivePositionDto, ElectivePosition>, IElectivePositionService
    {
        private readonly IGenericRepository<ElectivePosition> _positionRepository;
        private readonly IGenericRepository<Election> _electionRepository;
        private readonly IGenericRepository<CandidatePosition> _candidatePositionRepository;
        private readonly IGenericRepository<Candidate> _candidateRepository;

        public ElectivePositionService(
            IGenericRepository<ElectivePosition> positionRepository,
            IGenericRepository<Election> electionRepository,
            IGenericRepository<CandidatePosition> candidatePositionRepository,
            IGenericRepository<Candidate> candidateRepository,
            IMapper mapper)
            : base(positionRepository, mapper)
        {
            _positionRepository = positionRepository;
            _electionRepository = electionRepository;
            _candidatePositionRepository = candidatePositionRepository;
            _candidateRepository = candidateRepository;
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        public async Task<UpdateElectivePositionDto?> GetForEditAsync(int id)
        {
            var entity = await _positionRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<UpdateElectivePositionDto>(entity);
        }

        public async Task<OperationResult> CreateAsync(CreateElectivePositionDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede crear puestos electivos mientras exista una elección activa.");

            var entity = _mapper.Map<ElectivePosition>(dto);
            entity.IsActive = true;

            await _positionRepository.AddAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateAsync(UpdateElectivePositionDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede editar puestos electivos mientras exista una elección activa.");

            var entity = await _positionRepository.GetByIdAsync(dto.Id);
            if (entity == null)
                return OperationResult.Fail("El puesto electivo no existe.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;

            await _positionRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> ToggleActiveAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede cambiar el estado de un puesto mientras exista una elección activa.");

            var entity = await _positionRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("El puesto electivo no existe.");

            entity.IsActive = !entity.IsActive;

            // If the position is deactivated, cascade the change:
            // 1. Deactivate all assignments for this position
            // 2. Deactivate all candidates that were assigned to this position
            if (!entity.IsActive)
            {
                var assignments = await _candidatePositionRepository.GetAllAsync(
                    cp => cp.ElectivePositionId == entity.Id && cp.IsActive);

                var candidateIds = assignments.Select(a => a.CandidateId).Distinct().ToList();

                // Deactivate the assignments
                foreach (var assignment in assignments)
                {
                    assignment.IsActive = false;
                    await _candidatePositionRepository.UpdateAsync(assignment);
                }

                // Deactivate the candidates
                foreach (var candidateId in candidateIds)
                {
                    var candidate = await _candidateRepository.GetByIdAsync(candidateId);
                    if (candidate != null && candidate.IsActive)
                    {
                        candidate.IsActive = false;
                        await _candidateRepository.UpdateAsync(candidate);
                    }
                }
            }

            await _positionRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }
    }
}
