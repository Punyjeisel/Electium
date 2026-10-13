using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CreateCandidatePositionDto
    {
        public int CandidateId { get; set; }
        public int ElectivePositionId { get; set; }
        public int PoliticalPartyId { get; set; }
    }
}
