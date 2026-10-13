using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class VotingPositionDto
    {
        public int ElectivePositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public int PartyCount { get; set; }
        public int CandidateCount { get; set; }
    }
}
