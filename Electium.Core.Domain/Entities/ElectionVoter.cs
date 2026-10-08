using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

// snapshot of who participates in an election

namespace Electium.Core.Domain.Entities
{
    public class ElectionVoter : BaseEntity
    {
        public int ElectionId { get; set; }
        public Election Election { get; set; } = null!;
        
        public int CitizenId { get; set; }
        public Citizen Citizen { get; set; } = null!;
    }
}
