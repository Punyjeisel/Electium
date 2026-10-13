using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class AvailableCandidateDto
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PartyAcronym { get; set; } = string.Empty;
        public bool IsFromAlly { get; set; }
    }
}
