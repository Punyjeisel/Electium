using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CandidatePositionDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string CandidateFullName { get; set; } = string.Empty;
        public int ElectivePositionId { get; set; }
        public string ElectivePositionName { get; set; } = string.Empty;
        public int PoliticalPartyId { get; set; }
        public bool IsActive { get; set; }
    }
}
