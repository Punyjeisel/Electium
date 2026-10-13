using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

// snapshot of who participates in an election

namespace Electium.Core.Domain.Entities
{
    public class ElectionCandidate : BaseEntity
    {
        public int ElectionId { get; set; }
        public Election Election { get; set; } = null!;

        public int ElectivePositionId { get; set; }
        public ElectivePosition ElectivePosition { get; set; } = null!;
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;

        public int PoliticalPartyId { get; set; }
        public PoliticalParty PoliticalParty { get; set; } = null!;
    }
}
