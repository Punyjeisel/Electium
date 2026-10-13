using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class ElectionResultDto
    {
        public string PositionName { get; set; } = string.Empty;
        public int TotalVotesForPosition { get; set; }
        public List<CandidateResultDto> Candidates { get; set; } = new();
    }

    public class CandidateResultDto
    {
        public string CandidateName { get; set; } = string.Empty;
        public string PartyAcronym { get; set; } = string.Empty;
        public int Votes { get; set; }
        public double Percentage { get; set; }
    }
}
