using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class ElectivePositionService : GenericService<ElectivePositionDto, ElectivePosition>, IElectivePositionService
    {
        public ElectivePositionService(
            IGenericRepository<ElectivePosition> repository,
            IMapper mapper) 
            : base(repository, mapper)
        {
        }
    }
}
