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
    public class PartyLeaderService
    : GenericService<PartyLeaderDto, PartyLeader>, IPartyLeaderService
    {
        private readonly IGenericRepository<PartyLeader> _partyLeaderRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<PoliticalParty> _partyRepository;
        private readonly IGenericRepository<Election> _electionRepository;

        public PartyLeaderService(
            IGenericRepository<PartyLeader> partyLeaderRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<PoliticalParty> partyRepository,
            IGenericRepository<Election> electionRepository,
            IMapper mapper)
            : base(partyLeaderRepository, mapper)
        {
            _partyLeaderRepository = partyLeaderRepository;
            _userRepository = userRepository;
            _partyRepository = partyRepository;
            _electionRepository = electionRepository;
        }

        public async Task<List<PartyLeaderDto>> GetAllWithIncludesAsync()
        {
            // Only active relations are shown
            var leaders = await _partyLeaderRepository.GetAllAsync(
                pl => pl.IsActive,
                "User",
                "PoliticalParty");

            return _mapper.Map<List<PartyLeaderDto>>(leaders);
        }

        public async Task<bool> IsUserAlreadyAssignedAsync(int userId)
        {
            return await _partyLeaderRepository.AnyAsync(
                pl => pl.UserId == userId && pl.IsActive);
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        public async Task<OperationResult> CreateAsync(CreatePartyLeaderDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede crear asignaciones mientras exista una elección activa.");

            if (await IsUserAlreadyAssignedAsync(dto.UserId))
                return OperationResult.Fail(
                    "Este dirigente ya está relacionado con otro partido político.");

            var entity = _mapper.Map<PartyLeader>(dto);
            entity.IsActive = true;

            await _partyLeaderRepository.AddAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede eliminar asignaciones mientras exista una elección activa.");

            var entity = await _partyLeaderRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("La asignación no existe.");

            entity.IsActive = false;
            await _partyLeaderRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }

        // Leaders available to assign: active users with role PoliticalLeader
        // who are not already linked to a party
        public async Task<List<AvailableLeaderDto>> GetAvailableLeadersAsync()
        {
            var users = await _userRepository.GetAllAsync(u =>
                u.IsActive && u.Role == UserRole.PoliticalLeader);

            var result = new List<AvailableLeaderDto>();
            foreach (var user in users)
            {
                if (!await IsUserAlreadyAssignedAsync(user.Id))
                {
                    result.Add(new AvailableLeaderDto
                    {
                        UserId = user.Id,
                        FullName = $"{user.FirstName} {user.LastName}"
                    });
                }
            }

            return result;
        }

        public async Task<List<AvailablePartyDto>> GetActivePartiesAsync()
        {
            var parties = await _partyRepository.GetAllAsync(p => p.IsActive);

            return parties.Select(p => new AvailablePartyDto
            {
                Id = p.Id,
                Acronym = p.Acronym,
                Name = p.Name
            }).ToList();
        }
    }
}
