using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class VotingSummaryDto
    {
        public string CitizenName { get; set; } = string.Empty;
        public string CitizenEmail { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
        public DateTime ElectionDate { get; set; }
        public List<VotingSummaryItemDto> Items { get; set; } = new();
    }

    public class VotingSummaryItemDto
    {
        public string PositionName { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;   // "Ninguno" si aplica
        public string PartyAcronym { get; set; } = string.Empty;
    }
}
