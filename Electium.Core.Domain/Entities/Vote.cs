using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

// Anonymous vote. No citizen info here

namespace Electium.Core.Domain.Entities
{
    public class Vote : BaseEntity
    {
        public int ElectionId { get; set; }
        public Election Election { get; set; } = null!;

        public int ElectivePositionId { get; set; }
        public ElectivePosition ElectivePosition { get; set; } = null!;


        // Null whe the vote chose "Ninguno"
        public int? CandidateId { get; set; }
        public Candidate Candidate { get; set; }
    }
}
