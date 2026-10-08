using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

// Candidate asigned to a political party and an elective position

namespace Electium.Core.Domain.Entities
{
    public class CandidatePosition : BaseEntity
    {
        public int PoliticalPartyId { get; set; }
        public PoliticalParty PoliticalParty { get; set; } = null!;
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;
        public int ElectivePositionId { get; set; }
        public ElectivePosition ElectivePosition { get; set; } = null!;
    }
}
