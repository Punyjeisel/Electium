using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class ElectionSummaryDto
    {
        public string ElectionName { get; set; } = string.Empty;
        public int PartyCount { get; set; }
        public int CandidateCount { get; set; }
        public int TotalVotes { get; set; }
    }
}
