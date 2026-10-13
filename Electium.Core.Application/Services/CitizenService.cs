using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class CitizenService : GenericService<CitizenDto, Citizen>, ICitizenService
    {
        private readonly IGenericRepository<Citizen> _citizenRepository;

        public CitizenService(IGenericRepository<Citizen> citizenRepository, IMapper mapper) : base(citizenRepository, mapper)
        {
            _citizenRepository = citizenRepository;
        }

        public async Task<bool> IsIdentityNumberTakenAsync(
            string identityNumber,
            int? excludeCitizenId = null)
        {
            return await _citizenRepository.AnyAsync(c =>
            c.IdentityNumber == identityNumber &&
            (excludeCitizenId == null || c.Id != excludeCitizenId));
        }
    }
}
