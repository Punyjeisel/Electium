using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class LeaderDashboardDto
    {
        public string PartyName { get; set; } = string.Empty;
        public string PartyAcronym { get; set; } = string.Empty;
        public string PartyLogoPath { get; set; } = string.Empty;

        public int ActiveCandidates { get; set; }
        public int InactiveCandidates { get; set; }
        public int ActiveAlliances { get; set; }
        public int PendingReceivedRequests { get; set; }
        public int AssignedCandidates { get; set; }
    }
}
